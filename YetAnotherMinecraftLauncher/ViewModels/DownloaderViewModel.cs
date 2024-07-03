using System.Collections.ObjectModel;
using ReactiveUI;
using System.Reactive;
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
    }
}