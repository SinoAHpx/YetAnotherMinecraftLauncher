using System;
using System.Reactive;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;

namespace YetAnotherMinecraftLauncher.ViewModels;

public class MainViewModel : ViewModelBase
{
    #region Account

    private Bitmap _accountAvatar;

    public Bitmap AccountAvatar
    {
        get => _accountAvatar;
        set => this.RaiseAndSetIfChanged(ref _accountAvatar, value);
    }

    private string _accountName;

    public string AccountName
    {
        get => _accountName;
        set => this.RaiseAndSetIfChanged(ref _accountName, value);
    }

    private string _accountType;

    public string AccountType
    {
        get => _accountType;
        set => this.RaiseAndSetIfChanged(ref _accountType, value);
    }

    public ReactiveCommand<Unit, Unit> AccountActionCommand { get; set; }

    public void InteractAccount()
    {

    }

    #endregion

    #region Version

    private Bitmap _versionAvatar;

    public Bitmap VersionAvatar
    {
        get => _versionAvatar;
        set => this.RaiseAndSetIfChanged(ref _versionAvatar, value);
    }

    private string _versionName;

    public string VersionName
    {
        get => _versionName;
        set => this.RaiseAndSetIfChanged(ref _versionName, value);
    }

    private string _versionType;

    public string VersionType
    {
        get => _versionType;
        set => this.RaiseAndSetIfChanged(ref _versionType, value);
    }

    public ReactiveCommand<Unit, Unit> VersionActionCommand { get; set; }

    public void InteractVersion()
    {

    }

    #endregion
    public MainViewModel()
    {
        VersionActionCommand = ReactiveCommand.Create(InteractVersion);
        AccountActionCommand = ReactiveCommand.Create(InteractAccount);

        //some mocking values for diagnosing

        #region Version

        VersionType = "Vanilla";
        VersionName = "1.8.9";
        VersionAvatar = new(AssetLoader.Open(
            new Uri("avares://YetAnotherMinecraftLauncher/Assets/DefaultVersionAvatar.webp")));

        #endregion

        #region Account

        AccountType = "Microsoft";
        AccountName = "AHpx";
        AccountAvatar = new(AssetLoader.Open(
            new Uri("avares://YetAnotherMinecraftLauncher/Assets/DefaultAccountAvatar.png")));

        #endregion
    }

}
