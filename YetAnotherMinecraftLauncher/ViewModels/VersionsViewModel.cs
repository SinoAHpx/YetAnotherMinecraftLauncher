using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Manganese.IO;
using ModuleLauncher.NET.Utilities;
using YetAnotherMinecraftLauncher.Utils;
using YetAnotherMinecraftLauncher.Views.Controls;

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
            VersionsList.Add(new SelectiveItem
            {
                Avatar = new Bitmap(AssetLoader.Open(
                    new Uri("avares://YetAnotherMinecraftLauncher/Assets/DefaultVersionAvatar.webp"))),
                Title = minecraft.Tree.Jar.DirectoryName.GetFileName(),
                Subtitle = minecraft.GetMinecraftType().ToString()
            });
        }

    }

    #endregion
}