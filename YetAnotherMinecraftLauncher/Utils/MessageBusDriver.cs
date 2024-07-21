using System;
using System.Security.Cryptography.X509Certificates;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Models.Messages;

/// <summary>
/// MessageBus deliver
/// </summary>
public static class MessageBusDriver
{
    public static void Subscribe<T>(this MessageBusRoutes routes, Action<T> onNext)
    {
        MessageBus.Current.Listen<T>(routes.ToString()).Subscribe(onNext);
    }


    public static void DriveTo(this MessageBusRoutes routes)
    {
        switch (routes)
        {
            case MessageBusRoutes.ReturnToHome:
                ReturnToHome();
                break;
            case MessageBusRoutes.ToDownload:
                ToDownload();
                break;
            case MessageBusRoutes.UpdateVersions:
                UpdateVersion();
                break;
        }
    }

    public static void DriveToWith<T>(this MessageBusRoutes routes, T message)
    {
        switch (routes)
        {
            case MessageBusRoutes.RemoveAccount:
                RemoveAccount(message);
                break;
            case MessageBusRoutes.SelectAccount:
                SelectAccount(message);
                break;
            case MessageBusRoutes.RemoveVersion:
                RemoveVersion(message);
                break;
            case MessageBusRoutes.SelectVersion:
                SelectVersion(message);
                break;
        }
    }

    private static void UpdateVersion()
    {
        MessageBus.Current.SendMessage("", MessageBusRoutes.UpdateVersions.ToString());
    }

    private static void ReturnToHome()
    {
        MessageBus.Current.SendMessage("", MessageBusRoutes.ReturnToHome.ToString());
    }

    private static void ToDownload()
    {
        MessageBus.Current.SendMessage("",MessageBusRoutes.ToDownload.ToString());
    }

    private static void RemoveAccount<T>(T message)
    {
        MessageBus.Current.SendMessage(message, MessageBusRoutes.RemoveAccount.ToString());
    }

    private static void SelectAccount<T>(T message)
    {
        MessageBus.Current.SendMessage(message, MessageBusRoutes.SelectAccount.ToString());
    }

    private static void SelectVersion<T>(T message)
    {
        MessageBus.Current.SendMessage(message, MessageBusRoutes.SelectAccount.ToString());
    }

    private static void RemoveVersion<T>(T message)
    {
        MessageBus.Current.SendMessage(message, MessageBusRoutes.RemoveVersion.ToString());
    }
}