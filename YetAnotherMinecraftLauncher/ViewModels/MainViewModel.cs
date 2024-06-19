using System;
using System.Reactive;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Views;
using YetAnotherMinecraftLauncher.Views.Controls;

namespace YetAnotherMinecraftLauncher.ViewModels;

public class MainViewModel : ViewModelBase
{
    private int _mainViewIndex;

    public int MainViewIndex
    {
        get => _mainViewIndex;
        set => this.RaiseAndSetIfChanged(ref _mainViewIndex, value);
    }

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

    #endregion

    #region Expansive Section

    public ReactiveCommand<Unit, Unit> SettingActionCommand { get; set; }

    public ReactiveCommand<Unit, Unit> DownloadActionCommand { get; set; }

    public ReactiveCommand<Unit, Unit> LaunchActionCommand { get; set; }

    #endregion

    #region Actual logics

    public void InteractAccount()
    {
        //MainViewIndex = 2;
        MainViewIndex = 1;
    }


    public void InteractVersion()
    {
        MainViewIndex = 2;
    }
    public void InteractDownload()
    {
        MainViewIndex = 3;
    }

    public void InteractSetting()
    {
        MainViewIndex = 4;
    }

    public void InteractLaunch()
    {

    }

    #endregion

    public MainViewModel()
    {
        #region Register commands

        VersionActionCommand = ReactiveCommand.Create(InteractVersion);
        AccountActionCommand = ReactiveCommand.Create(InteractAccount);
        SettingActionCommand = ReactiveCommand.Create(InteractSetting);
        DownloadActionCommand = ReactiveCommand.Create(InteractDownload);
        LaunchActionCommand = ReactiveCommand.Create(InteractLaunch);

        #endregion

        //todo: some mocking values for diagnosing
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

        #region MessageBus receiver

        MessageBus.Current.Listen<string>().Subscribe(o =>
        {
            if (o.Contains("Return"))
            {
                MainViewIndex = 0;
            }
        });

        MessageBus.Current.Listen<SelectiveItem>().Subscribe(s =>
        {
            AccountAvatar = (Bitmap)s.Avatar;
            AccountName = s.Title;
            AccountType = s.Subtitle;
            AccountActionCommand = AccountActionCommand = ReactiveCommand.Create(InteractAccount);

            MainViewIndex = 0;
        });

        #endregion

    }
}
