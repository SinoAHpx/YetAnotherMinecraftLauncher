using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using DialogHostAvalonia;
using Downloader;
using DynamicData;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;
using MoreLinq;
using Polly.Retry;
using Polly;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Models.Messages;
using YetAnotherMinecraftLauncher.ViewModels.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher.Views.Controls.Dialogs
{
    public partial class DownloadingDialog : UserControl
    {
        public DownloadingDialog()
        {
            InitializeComponent();

            DataContext = _viewModel;
        }

        private readonly DownloadingDialogViewModel _viewModel = new();


        public async Task<DownloadingDialog> ShowDialogAsync(int totalProgress, Action cancelAction)
        {
            _viewModel.TotalProgress = totalProgress;
            _viewModel.CurrentProgress = 0;
            _viewModel.CancelCommand = ReactiveCommand.Create(cancelAction);
            await DialogHost.Show(this);
            
            return this;
        }


        public async Task DownloadAsync(List<(string url, string path)> downloadItems)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            _viewModel.CancelCommand = ReactiveCommand.Create(() =>
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                DialogHost.Close(null);
            });
            _viewModel.TotalProgress = 0;
            _viewModel.CurrentProgress = 0;
            DialogHost.Show(this);
            
            if (downloadItems.Count == 0)
            {
                DialogHost.Close(null);
                return;
            }
            var downloadBatches = downloadItems.Batch(10).ToList();
            SetTotalProgress(downloadBatches.Count);


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
                                MaxRetryAttempts = 10,
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
                    Update(progress++);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

            MessengerRoutes.UpdateVersions.Knock();
            DialogHost.Close(null);
        }

        public async Task<List<(string, string)>> GetDownloadItemsAsync(MinecraftEntry minecraft)
        {
            var libraries = minecraft.GetLibraries();
            var assets = await minecraft.GetAssetsAsync();

            var downloadItems = new List<(string url, string path)>();
            await Task.Run(() =>
            {
                downloadItems.Add(libraries.Where(x => !x.ValidateChecksum())
                    .Select(x => (x.GetDownloadUrl(), x.File.FullName)));
                downloadItems.Add(assets.Where(x => !x.ValidateChecksum())
                    .Select(x => (x.GetDownloadUrl(), x.File.FullName)));
                if (!minecraft.ValidateChecksum())
                {
                    downloadItems.Add((minecraft.GetDownloadUrl(), minecraft.Tree.Jar.FullName));
                }
            });

            return downloadItems;
        }

        public void SetTotalProgress(int totalProgress)
        {
            _viewModel.TotalProgress = totalProgress;
        }

        public void Update(int currentProgress)
        {
            _viewModel.CurrentProgress = currentProgress;
        }
    }
}
