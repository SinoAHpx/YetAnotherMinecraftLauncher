using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DialogHostAvalonia;
using Manganese.Process;
using Material.Dialog.Views;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Utilities;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Utils;
using YetAnotherMinecraftLauncher.Views;
using YetAnotherMinecraftLauncher.Views.Controls;
using AlertDialog = YetAnotherMinecraftLauncher.Views.Controls.Dialogs.AlertDialog;

namespace YetAnotherMinecraftLauncher.ViewModels;

public class MainViewModel : ViewModelBase
{
    private int _mainViewIndex;

    /// <summary>
    /// 0: MainView 1: AccountsView 2: VersionsView 3: DownloaderView 4: SettingsView
    /// </summary>
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

    private bool _onLaunching;

    public bool OnLaunching
    {
        get => _onLaunching;
        set => this.RaiseAndSetIfChanged(ref _onLaunching, value);
    }

    private string _launchingOutput;

    public string LaunchingOutput
    {
        get => _launchingOutput;
        set => this.RaiseAndSetIfChanged(ref _launchingOutput, value);
    }

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

    public async void InteractLaunch()
    {
        OnLaunching = true;

        if (!ConfigUtils.CheckConfig())
        {
            await new AlertDialog().ShowDialogAsync("Certain configs are not properly set.");
            InteractSetting();
            return;
        }

        //todo: this is pilot-only codes, needs more tweaks
        var resolver = ConfigUtils.GetMinecraftResolver();
        var mc = resolver.GetMinecraft(VersionName);
        var process = await mc
            .WithJava(new MinecraftJava
            {
                Executable = new FileInfo("C:\\Program Files\\Eclipse Adoptium\\jre-17.0.11.9-hotspot\\bin\\javaw.exe"),
                Version = 17
            })
            .WithJava(new MinecraftJava
            {
                Executable = new FileInfo(@"C:\Program Files\Eclipse Adoptium\jre-21.0.3.9-hotspot\bin\javaw.exe"),
                Version = 21
            })
            .WithAuthentication("AHpx")
            .WithLauncherName("YAML Pilot")
            .LaunchAsync();


        while (await process.ReadOutputLineAsync() is { } output)
        {
            LaunchingOutput = output;
        }

        OnLaunching = false;
        LaunchingOutput = string.Empty; 
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

        #region Navigation receivers

        MessageBus.Current.Listen<string>().Subscribe(o =>
        {
            if (o.Contains("Return"))
            {
                MainViewIndex = 0;
            }

            if (o.Contains("Download"))
            {
                MainViewIndex = 3;
            }
        });

        #endregion

        #region Accounts selection

        OfDefaultAccount();

        MessageBus.Current.Listen<SelectiveItem>("Accounts").Subscribe(s =>
        {
            AccountAvatar = (Bitmap)s.Avatar;
            AccountName = s.Title;
            AccountType = s.Subtitle;
            AccountActionCommand = ReactiveCommand.Create(InteractAccount);

            MainViewIndex = 0;
        });
        MessageBus.Current.Listen<SelectiveItem>("AccountRemoved").Subscribe(a =>
        {
            if (AccountName == a.Title)
            {
                OfDefaultAccount();
            }
        });


        #endregion

        #region Versions selection

        OfDefaultVersion();

        MessageBus.Current.Listen<SelectiveItem>("Versions").Subscribe(s =>
        {
            VersionAvatar = (Bitmap)s.Avatar;
            VersionName = s.Title;
            VersionType = s.Subtitle;
            VersionActionCommand = ReactiveCommand.Create(InteractVersion);

            MainViewIndex = 0;
        });
        MessageBus.Current.Listen<SelectiveItem>("VersionRemoved").Subscribe(a =>
        {
            if (VersionName == a.Title)
            {
                OfDefaultVersion();
            }
        });

        #endregion
    }

    private void OfDefaultAccount()
    {
        AccountType = "";
        AccountName = "Please Select";
        AccountAvatar = new(AssetLoader.Open(
            new Uri("avares://YetAnotherMinecraftLauncher/Assets/DefaultAccountAvatar.png")));
    }

    private void OfDefaultVersion()
    {
        VersionType = "";
        VersionName = "Please Select";
        VersionAvatar = new(AssetLoader.Open(
            new Uri("avares://YetAnotherMinecraftLauncher/Assets/DefaultVersionAvatar.webp")));
    }
}
