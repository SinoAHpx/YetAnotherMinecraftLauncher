﻿using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Manganese.Text;
using ModuleLauncher.NET.Utilities;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Models.Config;
using YetAnotherMinecraftLauncher.Models.Data;
using YetAnotherMinecraftLauncher.Models.Messages;
using YetAnotherMinecraftLauncher.Utils;
using YetAnotherMinecraftLauncher.Views.Controls;
using YetAnotherMinecraftLauncher.Views.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher.ViewModels;

public class VersionsViewModel : ViewModelBase
{
    private string _searchTerm;

    private int _selectedIndex;

    private ObservableCollection<SelectiveItem> _versionsList = [];

    public VersionsViewModel()
    {
        #region Register commands

        ReturnCommand = ReactiveCommand.Create(ReturnToHome);
        DownloadVersionCommand = ReactiveCommand.Create(DownloadVersion);

        #endregion

        #region Finding and updating versions

        UpdateVersions();

        MessengerRoutes.UpdateVersions.Subscribe<string>(_ => { UpdateVersions(); });

        #endregion

        #region Searching filter

        this.WhenAnyValue(x => x.SearchTerm).Subscribe(s =>
        {
            if (s is "" or null)
            {
                foreach (var item in VersionsList) item.IsVisible = true;
                return;
            }

            foreach (var item in VersionsList) item.IsVisible = item.Title.ToLower().Contains(s.ToLower());
        });

        #endregion

        #region Config

        SelectedIndex = ConfigUtils.ReadConfig("Index", ConfigNodes.Version)?.ToInt32() ?? -1;

        this.WhenAnyValue(x => x.SelectedIndex).Subscribe(async i =>
        {
            if (SelectedIndex == -1) return;

            var version = VersionsList[i];
            MessengerRoutes.SelectVersion.KnockWithMessage(version);

            await new
            {
                Index = SelectedIndex,
                Name = version.Title,
                Type = version.Subtitle,
                Avatar = "avares://YetAnotherMinecraftLauncher/Assets/DefaultVersionAvatar.webp"
            }.WriteConfigAsync(ConfigNodes.Version);
        });

        #endregion
    }

    public ReactiveCommand<Unit, Unit> ReturnCommand { get; set; }

    public ObservableCollection<SelectiveItem> VersionsList
    {
        get => _versionsList;
        set => this.RaiseAndSetIfChanged(ref _versionsList, value);
    }

    public string SearchTerm
    {
        get => _searchTerm;
        set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
    }


    public ReactiveCommand<Unit, Unit> DownloadVersionCommand { get; set; }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedIndex, value);
    }

    #region Sundry

    private void UpdateVersions()
    {
        VersionsList.Clear();
        var resolver = ConfigUtils.GetMinecraftResolver();
        if (resolver is null) return;
        var minecrafts = resolver.GetMinecrafts();
        foreach (var minecraft in minecrafts)
        {
            var id = minecraft.Tree.VersionRoot.Name;
            var item = new SelectiveItem
            {
                Avatar = DefaultAssets.VersionAvatar,
                Title = id,
                Subtitle = minecraft.GetMinecraftType().ToString()
            };
            item.SelectAction = ReactiveCommand.Create(() => SelectVersion(item));
            item.RemoveAction = ReactiveCommand.Create(() => RemoveVersion(item));

            VersionsList.Add(item);
        }
    }

    #endregion

    #region Logic

    //ReturnCommand
    public void ReturnToHome()
    {
        MessengerRoutes.ReturnToHome.Knock();
    }

    public void DownloadVersion()
    {
        MessengerRoutes.ToDownload.Knock();
    }

    public void SelectVersion(SelectiveItem item)
    {
        SelectedIndex = VersionsList.IndexOf(item);
        ReturnToHome();
    }

    public async void RemoveVersion(SelectiveItem item)
    {
        if (await new ConfirmDialog().ShowDialogAsync(
                "Removal will only involved with jar and json files, libraries and assets will be ignored. Are you sure about removing this version?"))
        {
            var resolver = ConfigUtils.GetMinecraftResolver();

            VersionsList.Remove(item);
            if (VersionsList.IndexOf(item) == SelectedIndex) SelectedIndex = -1;
            var mc = resolver!.GetMinecraft(item.Title);
            mc.Tree.VersionRoot.Delete(true);

            MessengerRoutes.RemoveVersion.KnockWithMessage(item);
        }
    }

    #endregion
}