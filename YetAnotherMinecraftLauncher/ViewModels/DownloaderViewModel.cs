using System;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Timers;
using Avalonia.Threading;
using Manganese.Array;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;
using YetAnotherMinecraftLauncher.Utils;
using YetAnotherMinecraftLauncher.Views.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DialogHostAvalonia;
using Manganese.Text;
using YetAnotherMinecraftLauncher.Views.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher.ViewModels;

public class DownloaderViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> ReturnCommand { get; set; }

    public ReactiveCommand<Unit, Unit> RefreshVersionsCommand { get; set; }


    private bool _showRelease = true;

    public bool ShowRelease
    {
        get => _showRelease;
        set => this.RaiseAndSetIfChanged(ref _showRelease, value);
    }

    private bool _showSnapshot = true;

    public bool ShowSnapshot
    {
        get => _showSnapshot;
        set => this.RaiseAndSetIfChanged(ref _showSnapshot, value);
    }

    private bool _showAncient = true;

    public bool ShowAncient
    {
        get => _showAncient;
        set => this.RaiseAndSetIfChanged(ref _showAncient, value);
    }

    public ObservableCollection<DownloadableItem> DownloadableVersions { get; set; } = [];

    private string _searchTerm;

    public string SearchTerm
    {
        get => _searchTerm;
        set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
    }

    #region Logic

    //ReturnCommand
    public void ReturnToHome()
    {
        MessageBus.Current.SendMessage(nameof(ReturnToHome));
    }

    public async void DownloadVersion(string id)
    {
        var dialog = new DownloadingDialog();
        dialog.ShowDialogAsync(1000, () =>
        {
            DialogHost.Close(null);
        });
        ;
        for (int i = 0; i < 1000; i++)
        {
            await Task.Delay(10);
            dialog.Update(i);
        }

        if (DialogHost.IsDialogOpen(null))
        {
            DialogHost.Close(null);

        }
    }

    private readonly string LocalVersionsManifestPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
        .CombinePath("version_manifest.json");

    public void RefreshVersions()
    {
        DownloaderUtils.RefreshLocalVersionsManifestAsync(LocalVersionsManifestPath);
    }

    #endregion

    public DownloaderViewModel()
    {
        #region Register commands

        ReturnCommand = ReactiveCommand.Create(ReturnToHome);
        RefreshVersionsCommand = ReactiveCommand.Create(RefreshVersions);

        #endregion

        #region Grab versions

        Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var minecrafts = await DownloaderUtils.GetRemoteMinecraftsAsync(LocalVersionsManifestPath);
                var avatar = new Bitmap(AssetLoader.Open(
                    new Uri("avares://YetAnotherMinecraftLauncher/Assets/DefaultVersionAvatar.webp")));

                foreach (var minecraft in minecrafts)
                {
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
                        DownloadAction = ReactiveCommand.Create(() => DownloadVersion(minecraft.Id))
                    });
                }
            });
        });

        #endregion



        #region Filters

        this.WhenAnyValue(x1 => x1.ShowRelease,
                x2 => x2.ShowSnapshot,
                x3 => x3.ShowAncient,
                x4 => x4.SearchTerm,
                (x1, x2, x3,x4) => new
                {
                    Release = x1,
                    Snapshot = x2,
                    Anciet = x3,
                    SearchTerm = x4
                })
            .Subscribe(x =>
            {
                if (x.SearchTerm is "" or null)
                {
                    foreach (var item in DownloadableVersions)
                    {
                        item.IsVisible = item.Subtitle switch
                        {
                            "Release" => ShowRelease,
                            "Snapshot" => ShowSnapshot,
                            "Ancient" => ShowAncient
                        };
                    }
                }
                else
                {
                    foreach (var item in DownloadableVersions)
                    {
                        item.IsVisible = (item.Subtitle switch
                        {
                            "Release" => x.Release,
                            "Snapshot" => x.Snapshot,
                            "Ancient" => x.Anciet,
                            _ => item.IsVisible
                        }) && item.Title.ToLower().Contains(x.SearchTerm.ToLower());
                    }
                }

            });

        #endregion




    }

    #region Sundry


    #endregion
}