using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
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
using Downloader;
using DynamicData;
using Manganese.IO;
using Manganese.Text;
using MoreLinq;
using Polly;
using Polly.Retry;
using YetAnotherMinecraftLauncher.Models.Messages;
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

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        var dialog = new DownloadingDialog();
        dialog.ShowDialogAsync(() =>
        {
            DialogHost.Close(null);
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
        });
        

        var libraries = localMinecraft.GetLibraries();
        var assets = await localMinecraft.GetAssetsAsync();

        var downloadItems = new List<(string url, string path)>
        {
            libraries.Where(x => !x.ValidateChecksum()).Select(x => (x.GetDownloadUrl(), x.File.FullName)),
            assets.Where(x => !x.ValidateChecksum()).Select(x => (x.GetDownloadUrl(), x.File.FullName))
        };
        if (!localMinecraft.ValidateChecksum())
        {
            downloadItems.Add((localMinecraft.GetDownloadUrl(), localMinecraft.Tree.Jar.FullName));
        }

        if (downloadItems.Count == 0)
        {
            DialogHost.Close(null);
            return;
        }
        var downloadBatches = downloadItems.Batch(10).ToList();
        dialog.SetTotalProgress(downloadBatches.Count);


        var progress = 1;
        foreach (var downloadBatch in downloadBatches)
        {
            try
            {
                await Parallel.ForEachAsync(downloadBatch, cancellationToken, async (tuple, token) =>
                {
                    var pipeline = new ResiliencePipelineBuilder()
                        .AddRetry(new RetryStrategyOptions
                        {
                            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                            Delay = TimeSpan.FromSeconds(3),
                            MaxRetryAttempts = int.MaxValue,
                            BackoffType = DelayBackoffType.Constant
                        })
                        .Build();

                    await pipeline.ExecuteAsync(async (_) =>
                    {
                        var downloader = new DownloadService();
                        Console.WriteLine($"{tuple.url}");
                        await downloader.DownloadFileTaskAsync(tuple.url, tuple.path);
                    }, token);
                });
                dialog.Update(progress++);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }

        MessengerRoutes.UpdateVersions.Knock();
        DialogHost.Close(null);
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
                        DownloadAction = ReactiveCommand.Create(() => DownloadVersion(minecraft))
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