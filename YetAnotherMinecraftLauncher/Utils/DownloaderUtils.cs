using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DialogHostAvalonia;
using Downloader;
using DynamicData;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Utilities;
using MoreLinq;
using Polly.Retry;
using Polly;
using YetAnotherMinecraftLauncher.Models.Messages;
using YetAnotherMinecraftLauncher.Views.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher.Utils
{
    public static class DownloaderUtils
    {
        public static async Task DownloadAsync(MinecraftEntry minecraft)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var dialog = new DownloadingDialog();
            dialog.ShowDialogAsync(() =>
            {
                DialogHost.Close(null);
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            });


            var libraries = minecraft.GetLibraries();
            var assets = await minecraft.GetAssetsAsync();

            var downloadItems = new List<(string url, string path)>
            {
                libraries.Where(x => !x.ValidateChecksum()).Select(x => (x.GetDownloadUrl(), x.File.FullName)),
                assets.Where(x => !x.ValidateChecksum()).Select(x => (x.GetDownloadUrl(), x.File.FullName))
            };
            if (!minecraft.ValidateChecksum())
            {
                downloadItems.Add((minecraft.GetDownloadUrl(), minecraft.Tree.Jar.FullName));
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
    }
}
