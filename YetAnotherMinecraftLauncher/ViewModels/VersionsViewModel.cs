using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DialogHostAvalonia;
using Manganese.Array;
using Manganese.IO;
using ModuleLauncher.NET.Utilities;
using YetAnotherMinecraftLauncher.Utils;
using YetAnotherMinecraftLauncher.Views.Controls;
using YetAnotherMinecraftLauncher.Views.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher.ViewModels;

public class VersionsViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> ReturnCommand { get; set; }

    private ObservableCollection<SelectiveItem> _versionsList = [];

    public ObservableCollection<SelectiveItem> VersionsList
    {
        get => _versionsList;
        set => this.RaiseAndSetIfChanged(ref _versionsList, value);
    }

    private string _searchTerm;

    public string SearchTerm
    {
        get => _searchTerm;
        set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
    }


    public ReactiveCommand<Unit, Unit> DownloadVersionCommand { get; set; }
    #region Logic

    //ReturnCommand
    public void ReturnToHome()
    {
        MessageBus.Current.SendMessage(nameof(ReturnToHome));
    }

    public void DownloadVersion()
    {
        MessageBus.Current.SendMessage(nameof(DownloadVersion));
    }

    public void SelectVersion(SelectiveItem item)
    {
        MessageBus.Current.SendMessage(item, "Versions");
    }

    public async void RemoveVersion(SelectiveItem item)
    {
        if (await new ConfirmDialog().ShowDialogAsync("Removal will only involved with jar and json files, libraries and assets will be ignored. Are you sure about removing this account?"))
        {
            VersionsList.Remove(item);
            var resolver = ConfigUtils.GetMinecraftResolver();
            var mc = resolver.GetMinecraft(item.Title);
            mc.Tree.VersionRoot.Delete(true);

            MessageBus.Current.SendMessage(item,"VersionRemoved");
        }
    }

    #endregion

    public VersionsViewModel()
    {
        #region Register commands

        ReturnCommand = ReactiveCommand.Create(ReturnToHome);
        DownloadVersionCommand = ReactiveCommand.Create(DownloadVersion);

        #endregion

        #region Finding and updating versions

        UpdateVersions();

        MessageBus.Current.Listen<string>("UpdateVersions").Subscribe(_ =>
        {
            UpdateVersions();
        });

        #endregion

        #region Searching filter

        this.WhenAnyValue(x => x.SearchTerm).Subscribe(s =>
        {
            if (s is "" or null)
            {
                foreach (var item in VersionsList)
                {
                    item.IsVisible = true;
                }
                return;
            }

            foreach (var item in VersionsList)
            {
                item.IsVisible = item.Title.ToLower().Contains(s.ToLower());
            }
        });

        #endregion

    }

    #region Sundry

    private void UpdateVersions()
    {
        VersionsList.Clear();
        var resolver = ConfigUtils.GetMinecraftResolver();
        var minecrafts = resolver.GetMinecrafts();
        foreach (var minecraft in minecrafts)
        {
            var id = minecraft.Tree.VersionRoot.Name;
            var item = new SelectiveItem
            {
                Avatar = new Bitmap(AssetLoader.Open(
                    new Uri("avares://YetAnotherMinecraftLauncher/Assets/DefaultVersionAvatar.webp"))),
                Title = id,
                Subtitle = minecraft.GetMinecraftType().ToString(),

            };
            item.SelectAction = ReactiveCommand.Create(() => SelectVersion(item));
            item.RemoveAction = ReactiveCommand.Create(() => RemoveVersion(item));

            VersionsList.Add(item);
        }

    }

    #endregion
}