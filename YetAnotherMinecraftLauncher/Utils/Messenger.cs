using System.Security.Cryptography.X509Certificates;
using ReactiveUI;

/// <summary>
/// MessageBus deliver
/// </summary>
public static class Messenger
{
    public static void UpdateVersion<T>(T message)
    {
        MessageBus.Current.SendMessage(message, "UpdateVersions");
    }

    public static void ReturnToHome()
    {
        MessageBus.Current.SendMessage(nameof(ReturnToHome));
    }

    public static void AccountRemoved<T>(T message)
    {
        MessageBus.Current.SendMessage(message, "AccountRemoved");
    }

    public static void AccountSelected<T>(T message)
    {
        MessageBus.Current.SendMessage(message, "AccountSelected");
    }

    public static void VersionSelected<T>(T message)
    {
        MessageBus.Current.SendMessage(message, "VersionSelected");
    }

    public static void VersionRemoved<T>(T message)
    {
        MessageBus.Current.SendMessage(message, "VersionRemoved");
    }
}