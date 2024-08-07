﻿using System;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Models.Messages;

/// <summary>
///     MessageBus deliver
/// </summary>
public static class Messenger
{
    public static void Subscribe<T>(this MessengerRoutes routes, Action<T> onNext)
    {
        MessageBus.Current.Listen<T>(routes.ToString()).Subscribe(onNext);
    }


    public static void Knock(this MessengerRoutes routes)
    {
        switch (routes)
        {
            case MessengerRoutes.ReturnToHome:
                ReturnToHome();
                break;
            case MessengerRoutes.ToDownload:
                ToDownload();
                break;
            case MessengerRoutes.UpdateVersions:
                UpdateVersions();
                break;
            case MessengerRoutes.UpdateAccounts:
                UpdateAccounts();
                break;
        }
    }


    public static void KnockWithMessage<T>(this MessengerRoutes routes, T message)
    {
        switch (routes)
        {
            case MessengerRoutes.RemoveAccount:
                RemoveAccount(message);
                break;
            case MessengerRoutes.SelectAccount:
                SelectAccount(message);
                break;
            case MessengerRoutes.RemoveVersion:
                RemoveVersion(message);
                break;
            case MessengerRoutes.SelectVersion:
                SelectVersion(message);
                break;
            // case MessengerRoutes.ReadAccount:
            //     ReadAccount(message);
            //     break;
            // case MessengerRoutes.ReadVersion:
            //     ReadVersion(message);
            //     break;
        }
    }

    // private static void ReadVersion<T>(T message)
    // {
    //     MessageBus.Current.SendMessage(message, MessengerRoutes.ReadVersion.ToString());
    // }
    //
    // private static void ReadAccount<T>(T message)
    // {
    //     MessageBus.Current.SendMessage(message, MessengerRoutes.ReadAccount.ToString());
    // }

    private static void UpdateAccounts()
    {
        MessageBus.Current.SendMessage("", MessengerRoutes.UpdateAccounts.ToString());
    }

    private static void UpdateVersions()
    {
        MessageBus.Current.SendMessage("", MessengerRoutes.UpdateVersions.ToString());
    }

    private static void ReturnToHome()
    {
        MessageBus.Current.SendMessage("", MessengerRoutes.ReturnToHome.ToString());
    }

    private static void ToDownload()
    {
        MessageBus.Current.SendMessage("", MessengerRoutes.ToDownload.ToString());
    }

    private static void RemoveAccount<T>(T message)
    {
        MessageBus.Current.SendMessage(message, MessengerRoutes.RemoveAccount.ToString());
    }

    private static void SelectAccount<T>(T message)
    {
        MessageBus.Current.SendMessage(message, MessengerRoutes.SelectAccount.ToString());
    }

    private static void SelectVersion<T>(T message)
    {
        MessageBus.Current.SendMessage(message, MessengerRoutes.SelectVersion.ToString());
    }

    private static void RemoveVersion<T>(T message)
    {
        MessageBus.Current.SendMessage(message, MessengerRoutes.RemoveVersion.ToString());
    }
}