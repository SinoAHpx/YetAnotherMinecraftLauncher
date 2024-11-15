using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Threading;
using Manganese.Text;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Models.Data;
using YetAnotherMinecraftLauncher.Models.Messages;
using YetAnotherMinecraftLauncher.Utils;
using YetAnotherMinecraftLauncher.Views.Controls;
using YetAnotherMinecraftLauncher.Views.Controls.Dialogs;
using DownloaderUtils = ModuleLauncher.NET.Utilities.DownloaderUtils;

namespace YetAnotherMinecraftLauncher.ViewModels;

public class DownloaderViewModel : ViewModelBase
{
    private string _searchTerm;

    private bool _showAncient = true;


    private bool _showRelease = true;

    private bool _showSnapshot = true;

    public DownloaderViewModel()
    {
        #region Register commands

        ReturnCommand = ReactiveCommand.Create(ReturnToHome);
        RefreshVersionsCommand = ReactiveCommand.Create(RefreshVersions);

        #endregion

        #region Grab versions

        Task.Run(async () =>
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var minecrafts = await DownloaderUtils.GetRemoteMinecraftsAsync(LocalVersionsManifestPath);
                var avatar = DefaultAssets.VersionAvatar;

                foreach (var minecraft in minecrafts)
                    DownloadableVersions.Add(new DownloadableItem
                    {
                        Title = minecraft.Id,
                        Subtitle = minecraft.Type switch
                        {
                            MinecraftJsonType.Release => "Release",
                            MinecraftJsonType.Snapshot => "Snapshot",
                            MinecraftJsonType.OldAlpha or MinecraftJsonType.OldBeta => "Ancient"
                        },
                        Avatar = avatar,
                        DownloadAction = ReactiveCommand.Create(() => DownloadVersion(minecraft))
                    });
            });
        });

        #endregion


        #region Filters

        this.WhenAnyValue(x1 => x1.ShowRelease,
                x2 => x2.ShowSnapshot,
                x3 => x3.ShowAncient,
                x4 => x4.SearchTerm,
                (x1, x2, x3, x4) => new
                {
                    Release = x1,
                    Snapshot = x2,
                    Anciet = x3,
                    SearchTerm = x4
                })
            .Subscribe(x =>
            {
                if (x.SearchTerm is "" or null)
                    foreach (var item in DownloadableVersions)
                        item.IsVisible = item.Subtitle switch
                        {
                            "Release" => ShowRelease,
                            "Snapshot" => ShowSnapshot,
                            "Ancient" => ShowAncient
                        };
                else
                    foreach (var item in DownloadableVersions)
                        item.IsVisible = item.Subtitle switch
                        {
                            "Release" => x.Release,
                            "Snapshot" => x.Snapshot,
                            "Ancient" => x.Anciet,
                            _ => item.IsVisible
                        } && item.Title.ToLower().Contains(x.SearchTerm.ToLower());
            });

        #endregion
    }

    public ReactiveCommand<Unit, Unit> ReturnCommand { get; set; }

    public ReactiveCommand<Unit, Unit> RefreshVersionsCommand { get; set; }

    public bool ShowRelease
    {
        get => _showRelease;
        set => this.RaiseAndSetIfChanged(ref _showRelease, value);
    }

    public bool ShowSnapshot
    {
        get => _showSnapshot;
        set => this.RaiseAndSetIfChanged(ref _showSnapshot, value);
    }

    public bool ShowAncient
    {
        get => _showAncient;
        set => this.RaiseAndSetIfChanged(ref _showAncient, value);
    }

    public ObservableCollection<DownloadableItem> DownloadableVersions { get; set; } = [];

    public string SearchTerm
    {
        get => _searchTerm;
        set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
    }

    #region Logic

    //ReturnCommand
    public void ReturnToHome()
    {
        MessengerRoutes.ReturnToHome.Knock();
    }

    public async void DownloadVersion(RemoteMinecraftEntry remoteMinecraft)
    {
        var resolver = ConfigUtils.GetMinecraftResolver();
        if (resolver is null)
        {
            await new AlertDialog().ShowDialogAsync("Maybe you should check your Minecraft directory is properly set?");
            return;
        }

        var localMinecraft = await remoteMinecraft.ResolveLocalEntryAsync(resolver);

        var downloadingDialog = new DownloadingDialog();
        var downloadingItems = await downloadingDialog.GetDownloadItemsAsync(localMinecraft);
        if (downloadingItems.Count != 0)
            await new DownloadingDialog().DownloadAsync(downloadingItems);
        else
            await new AlertDialog().ShowDialogAsync("This version has been already downloaded.");
    }

    private readonly string LocalVersionsManifestPath = Environment
        .GetFolderPath(Environment.SpecialFolder.ApplicationData)
        .CombinePath("version_manifest.json");

    public void RefreshVersions()
    {
        DownloaderUtils.RefreshLocalVersionsManifestAsync(LocalVersionsManifestPath);
    }

    #endregion
}