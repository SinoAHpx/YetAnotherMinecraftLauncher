using System;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DialogHostAvalonia;
using Manganese.Process;
using ModuleLauncher.NET.Authentications;
using ReactiveUI;
using WebViewControl;
using YetAnotherMinecraftLauncher.Utils;
using YetAnotherMinecraftLauncher.Views.Controls;
using YetAnotherMinecraftLauncher.Views.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher.ViewModels.Controls.Dialogs;

public class AddAccountDialogViewModel : ViewModelBase
{
    #region Offline username

    private string _offlineUsername;
    public string OfflineUsername
    {
        get => _offlineUsername;
        set => this.RaiseAndSetIfChanged(ref _offlineUsername, value);
    }

    public ReactiveCommand<Unit, Unit> AddOfflineUserCommand { get; set; }

    #endregion

    #region Microsoft

    public ReactiveCommand<Unit, Unit> AddMicrosoftUserCommand { get; set; }

    #endregion

    #region Actual logic

    public void AddOfflineUser()
    {
        var item = new SelectiveItem
        {
            Avatar = new Bitmap(AssetLoader.Open(
                new Uri("avares://YetAnotherMinecraftLauncher/Assets/DefaultAccountAvatar.png"))),
            Title = OfflineUsername,
            Subtitle = "Offline"
        };
        DialogHost.Close(null, item);
    }

    //todo: this is not completed
    public void AddMicrosoftUser()
    {
        var msAuthenticator = new MicrosoftAuthenticator();
        var mainWindow = LifetimeUtils.GetMainWindow();
        var loginWindow = new MicrosoftLoginWindow(msAuthenticator.LoginUrl);

        loginWindow.ShowDialog(mainWindow);

        MessageBus.Current
            .Listen<string>(nameof(MicrosoftLoginWindowViewModel))
            .Subscribe(async c =>
        {
            loginWindow.Close();
            msAuthenticator.Code = c;
            var authResult = await msAuthenticator.AuthenticateAsync();
            if (authResult != null)
            {
                var item = new SelectiveItem
                {
                    Avatar = new Bitmap(AssetLoader.Open(
                        new Uri("avares://YetAnotherMinecraftLauncher/Assets/DefaultAccountAvatar.png"))),
                    Title = authResult.Name,
                    Subtitle = "Offline"
                };
                DialogHost.Close(null, item);
            }
        });
    }

    #endregion

    public AddAccountDialogViewModel()
    {
        #region Reg cmd

        AddOfflineUserCommand = ReactiveCommand.Create(AddOfflineUser);
        AddMicrosoftUserCommand = ReactiveCommand.Create(AddMicrosoftUser);

        #endregion
    }
}