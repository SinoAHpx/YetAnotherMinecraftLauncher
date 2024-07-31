using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DialogHostAvalonia;
using Flurl.Http;
using Manganese.Process;
using Manganese.Text;
using ModuleLauncher.NET.Authentications;
using ModuleLauncher.NET.Models.Authentication;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Models.Messages;
using YetAnotherMinecraftLauncher.Utils;
using YetAnotherMinecraftLauncher.Views.Controls;
using YetAnotherMinecraftLauncher.Views.Controls.Dialogs;
using AlertDialog = Material.Dialog.Views.AlertDialog;

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

    public async void AddOfflineUser()
    {
        await AccountUtils.WriteAsync(OfflineUsername);
        MessengerRoutes.UpdateAccounts.Knock();
        DialogHost.Close(null);
    }

    public async void AddMicrosoftUser()
    {
        var mainWindow = LifetimeUtils.GetMainWindow();

        var authenticationResult = await LoginAsync();
        if (authenticationResult is null)
        {
            await new AlertDialog { Title = "Failed", Content = "Failed to authenticate.", WindowStartupLocation = WindowStartupLocation.CenterOwner }.ShowDialog(mainWindow);
            return;
        }

        var avatarBytes =
            await $"https://minotar.net/avatar/{authenticationResult.Name}".GetBytesAsync();
        
        
        var avatar = new Bitmap(new MemoryStream(avatarBytes));

        await AccountUtils.WriteAsync(authenticationResult);
        MessengerRoutes.UpdateAccounts.Knock();
        DialogHost.Close(null);
    }

    private void WriteAvatarAsync(AuthenticateResult result)
    {

    }

    /// <summary>
    /// Perform authentication with Microsoft account
    /// </summary>
    /// <returns></returns>
    private async Task<AuthenticateResult?> LoginAsync()
    {
        try
        {
            var mainWindow = LifetimeUtils.GetMainWindow();
            var ms = new MicrosoftAuthenticator()
            {
                //What can I say, Client ID out!
                ClientId = "831dc94c-7e4f-4ef6-b2e7-8fc1ec498111"
            };
            var deviceCode = await ms.GetDeviceCodeAsync();
            if (deviceCode is null)
            {
                await new AlertDialog { Title = "Failed", Content = "Failed to get a device code.", WindowStartupLocation = WindowStartupLocation.CenterOwner }.ShowDialog(mainWindow);
                return null;
            }

            deviceCode.VerificationUrl.OpenUrl();
            await LifetimeUtils.GetMainWindow().Clipboard!.SetTextAsync(deviceCode.UserCode);
            var webAuthorization = await ms.PollAuthorizationAsync(deviceCode);
            if (webAuthorization.accessToken.IsNullOrEmpty() || webAuthorization.refreshToken.IsNullOrEmpty())
            {
                await new AlertDialog { Title = "Failed", Content = "Failed to authorize.", WindowStartupLocation = WindowStartupLocation.CenterOwner }.ShowDialog(mainWindow);
                return null;
            }
            var authenticationResult = await ms.AuthenticateAsync(webAuthorization);

            return authenticationResult;
        }
        catch
        {
            return null;
        }
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