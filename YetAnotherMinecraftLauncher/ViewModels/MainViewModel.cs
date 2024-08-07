﻿using System;
using System.IO;
using System.Reactive;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Manganese.Process;
using Manganese.Text;
using ModuleLauncher.NET.Launcher;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Models.Config;
using YetAnotherMinecraftLauncher.Models.Data;
using YetAnotherMinecraftLauncher.Models.Messages;
using YetAnotherMinecraftLauncher.Utils;
using YetAnotherMinecraftLauncher.Views.Controls;
using YetAnotherMinecraftLauncher.Views.Controls.Dialogs;
using AlertDialog = YetAnotherMinecraftLauncher.Views.Controls.Dialogs.AlertDialog;

namespace YetAnotherMinecraftLauncher.ViewModels;

public class MainViewModel : ViewModelBase
{
    private int _mainViewIndex;

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

        MessengerRoutes.ReturnToHome.Subscribe<string>(_ => { MainViewIndex = 0; });

        MessengerRoutes.ToDownload.Subscribe<string>(_ => { MainViewIndex = 3; });

        #endregion

        #region Accounts selection

        ReadAccount();

        MessengerRoutes.SelectAccount.Subscribe<SelectiveItem>(SelectAccount);
        MessengerRoutes.RemoveAccount.Subscribe<SelectiveItem>(a =>
        {
            if (AccountName == a.Title) OfDefaultAccount();
        });

        #endregion

        #region Versions selection

        ReadVersion();

        MessengerRoutes.SelectVersion.Subscribe<SelectiveItem>(SelectVersion);
        MessengerRoutes.RemoveVersion.Subscribe<SelectiveItem>(a =>
        {
            if (VersionName == a.Title) OfDefaultVersion();
        });

        #endregion
    }

    /// <summary>
    ///     0: MainView 1: AccountsView 2: VersionsView 3: DownloaderView 4: SettingsView
    /// </summary>
    public int MainViewIndex
    {
        get => _mainViewIndex;
        set => this.RaiseAndSetIfChanged(ref _mainViewIndex, value);
    }

    private void OfDefaultAccount()
    {
        AccountType = "";
        AccountName = "Please Select";
        AccountAvatar = DefaultAssets.AccountAvatar;
    }

    private void SelectAccount(SelectiveItem s)
    {
        AccountAvatar = (Bitmap)s.Avatar;
        AccountName = s.Title;
        AccountType = s.Subtitle;
        AccountActionCommand = ReactiveCommand.Create(InteractAccount);

        MainViewIndex = 0;
    }

    private void ReadAccount()
    {
        var config = ConfigUtils.ReadConfig("Index", ConfigNodes.Account);
        if (config.IsNullOrEmpty())
        {
            OfDefaultAccount();
            return;
        }

        AccountName = ConfigUtils.ReadConfig("Name", ConfigNodes.Account) ?? "Please Select";
        AccountType = ConfigUtils.ReadConfig("Type", ConfigNodes.Account) ?? "";
        var avatarPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
            .CombinePath("YAML", $"{AccountName}.png");
        AccountAvatar = File.Exists(avatarPath)
            ? new Bitmap(File.OpenRead(avatarPath))
            : DefaultAssets.AccountAvatar;
    }

    private void OfDefaultVersion()
    {
        VersionType = "";
        VersionName = "Please Select";
        VersionAvatar = DefaultAssets.VersionAvatar;
    }


    private void SelectVersion(SelectiveItem s)
    {
        VersionAvatar = (Bitmap)s.Avatar;
        VersionName = s.Title;
        VersionType = s.Subtitle;
        VersionActionCommand = ReactiveCommand.Create(InteractVersion);
    }

    private void ReadVersion()
    {
        var config = ConfigUtils.ReadConfig("Index", ConfigNodes.Version);
        if (config.IsNullOrEmpty())
        {
            OfDefaultVersion();
            return;
        }

        VersionName = ConfigUtils.ReadConfig("Name", ConfigNodes.Version) ?? "Please Select";
        VersionType = ConfigUtils.ReadConfig("Type", ConfigNodes.Version) ?? "";
        VersionAvatar = new Bitmap(AssetLoader.Open(
            new Uri(ConfigUtils.ReadConfig("Avatar", ConfigNodes.Version) ??
                    "avares://YetAnotherMinecraftLauncher/Assets/DefaultVersionAvatar.webp")));
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

        var launchConfig = ConfigUtils.GetLauncherConfig();
        var resolver = ConfigUtils.GetMinecraftResolver();
        var javas = ConfigUtils.GetJavas();


        #region Check configs

        if (launchConfig is null)
        {
            await new AlertDialog().ShowDialogAsync("Certain configs are not properly set.");
            InteractSetting();
            goto end;
        }

        if (resolver is null)
        {
            await new AlertDialog().ShowDialogAsync("Certain configs are not properly set.");
            InteractSetting();
            goto end;
        }

        if (VersionName.IsNullOrEmpty() || VersionType.IsNullOrEmpty())
        {
            await new AlertDialog().ShowDialogAsync("Please select a version to launch.");
            InteractVersion();
            goto end;
        }

        //todo: account selection is not completed
        if (AccountName.IsNullOrEmpty() || AccountType.IsNullOrEmpty())
        {
            await new AlertDialog().ShowDialogAsync("Please select a account to launch.");
            InteractAccount();
            goto end;
        }

        if (javas.Count == 0)
        {
            await new AlertDialog().ShowDialogAsync("Please add java executable files.");
            InteractSetting();
            goto end;
        }

        #endregion

        launchConfig.Authentication = await AccountUtils.GetAccountAsync(AccountName) ?? "Offline";

        //todo: this could be wrong tho, waiting for upstream to fix this
        var minecraft = resolver.GetMinecraft(VersionName);

        #region Resource completion

        var downloadingDialog = new DownloadingDialog();
        var downloadingItems = await downloadingDialog.GetDownloadItemsAsync(minecraft);
        if (downloadingItems.Count != 0) await new DownloadingDialog().DownloadAsync(downloadingItems);

        #endregion

        #region Launching

        var launcher = new Launcher(resolver, launchConfig);
        var process = await launcher.LaunchAsync(minecraft);

        while (await process.ReadOutputLineAsync() is { } output) LaunchingOutput = output;

        #endregion

        end:
        OnLaunching = false;
        LaunchingOutput = string.Empty;
    }

    #endregion
}