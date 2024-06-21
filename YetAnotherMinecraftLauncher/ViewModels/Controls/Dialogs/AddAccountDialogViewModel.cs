using System;
using System.Reactive;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DialogHostAvalonia;
using Manganese.Process;
using ModuleLauncher.NET.Authentications;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Views.Controls;

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

    public void AddMicrosoftUser()
    {
        var msAuthenticator = new MicrosoftAuthenticator
        {
            ClientId = "831dc94c-7e4f-4ef6-b2e7-8fc1ec498111",
            RedirectUrl = "yaml://authms"
        };
        msAuthenticator.LoginUrl.OpenUrl();
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