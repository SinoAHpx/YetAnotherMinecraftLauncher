namespace YetAnotherMinecraftLauncher.Models.Messages;

public enum MessengerRoutes
{
    //navigation
    ReturnToHome,
    ToDownload,

    //behavior
    UpdateVersions,
    UpdateAccounts,

    //response
    RemoveAccount,
    SelectAccount,
    RemoveVersion,
    SelectVersion
}