using System;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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

        //this is some mocking values

        for (int i = 0; i < 10; i++)
        {
            VersionsList.Add(new SelectiveItem
            {
                Avatar = new Bitmap(AssetLoader.Open(
                    new Uri("avares://YetAnotherMinecraftLauncher/Assets/DefaultVersionAvatar.webp"))),
                Title = $"Version: {i}",
                Subtitle = $"Vanilla - {i * 10} hours"
            });
        }
    }
}