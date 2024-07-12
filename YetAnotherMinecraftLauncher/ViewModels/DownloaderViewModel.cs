using System;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using Manganese.Array;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;
using YetAnotherMinecraftLauncher.Utils;
using YetAnotherMinecraftLauncher.Views.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DialogHostAvalonia;
using YetAnotherMinecraftLauncher.Views.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher.ViewModels;

public class DownloaderViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> ReturnCommand { get; set; }

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

    public void DownloadVersion(string id)
    {
        DialogHost.Show(new AlertDialog
        {
            Message = $"The version to be downloaded is: {id}",
            DismissActionCommand = ReactiveCommand.Create(() =>
            {
                DialogHost.Close(null);
            })
        });
    }

    #endregion

    public DownloaderViewModel()
    {
        #region Register commands

        ReturnCommand = ReactiveCommand.Create(ReturnToHome);

        #endregion

        #region Grab versions

        Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var minecrafts = await DownloaderUtils.GetRemoteMinecraftsAsync();
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