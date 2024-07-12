using System;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Manganese.Array;
using ModuleLauncher.NET.Utilities;
using YetAnotherMinecraftLauncher.Utils;
using YetAnotherMinecraftLauncher.Views.Controls;

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

    #endregion

    public DownloaderViewModel()
    {
        #region Register commands

        ReturnCommand = ReactiveCommand.Create(ReturnToHome);

        #endregion

        #region Grab versions

        

        #endregion

        //mocking
        for (int i = 0; i < 100; i++)
        {
            DownloadableVersions.Add(new DownloadableItem
            {
                Title = $"Minecraft {i}",
                Subtitle = (new[] {"Release", "Snapshot", "Ancient"}).Random(),
                DownloadAction = ReactiveCommand.Create(() => {}) 
            });
        }

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
                        }) && item.Title.Contains(x.SearchTerm);
                    }
                }

            });

        #endregion




    }
}