using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using DialogHostAvalonia;
using Manganese.Text;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Models.Config;
using YetAnotherMinecraftLauncher.Models.Messages;
using YetAnotherMinecraftLauncher.Utils;
using YetAnotherMinecraftLauncher.Views.Controls;
using YetAnotherMinecraftLauncher.Views.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher.ViewModels;

internal class AccountsViewModel : ViewModelBase
{
    private ObservableCollection<SelectiveItem> _accountsList = [];


    private int _selectedIndex;

    public AccountsViewModel()
    {
        #region Register commands

        ReturnCommand = ReactiveCommand.Create(ReturnToHome);
        AddAccountCommand = ReactiveCommand.Create(AddAccount);

        #endregion

        UpdateAccounts();

        MessengerRoutes.UpdateAccounts.Subscribe<string>(_ => { UpdateAccounts(); });

        if (ConfigUtils.ReadConfig("Index", ConfigNodes.Account) is {} index)
        {
            SelectedIndex = index.ToInt32();
        }
        // SelectedIndex = ?.ToInt32() ?? -1;

        this.WhenAnyValue(x => x.SelectedIndex).Subscribe(async i =>
        {
            if (SelectedIndex == -1 || AccountsList.Count == 0) return;

            var account = AccountsList[i];
            MessengerRoutes.SelectAccount.KnockWithMessage(account);
            var avatarPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                .CombinePath("YAML");
            Directory.CreateDirectory(avatarPath);
            avatarPath = avatarPath.CombinePath($"{account.Title}.png");
            await new
            {
                Index = SelectedIndex,
                Name = account.Title,
                Type = account.Subtitle,
                Avatar = avatarPath
            }.WriteConfigAsync(ConfigNodes.Account);
        });
    }

    public ReactiveCommand<Unit, Unit> ReturnCommand { get; set; }

    public ObservableCollection<SelectiveItem> AccountsList
    {
        get => _accountsList;
        set => this.RaiseAndSetIfChanged(ref _accountsList, value);
    }

    public ReactiveCommand<Unit, Unit> AddAccountCommand { get; set; }

    public int SelectedIndex
    {
        get => _selectedIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedIndex, value);
    }

    private async void UpdateAccounts()
    {
        var accounts = await AccountUtils.ReadAsync();
        //shout we use refresh token here?

        if (accounts.Count == 0) return;

        AccountsList.Clear();
        foreach (var account in accounts)
        {
            var item = account.ToSelectiveItem();
            item.SelectAction = ReactiveCommand.Create(() => { SelectAccount(item); });
            item.RemoveAction = ReactiveCommand.Create(() => { RemoveAccount(item); });

            AccountsList.Add(item);
        }
    }

    #region Actual logics

    //ReturnCommand
    public void ReturnToHome()
    {
        MessengerRoutes.ReturnToHome.Knock();
    }

    public async void RemoveAccount(SelectiveItem item)
    {
        if (await new ConfirmDialog().ShowDialogAsync("Deletion cannot be undone"))
        {
            AccountsList.Remove(item);
            await AccountUtils.RemoveAsync(item.Title);

            MessengerRoutes.RemoveAccount.KnockWithMessage(item);
        }
    }

    public void SelectAccount(SelectiveItem item)
    {
        MessengerRoutes.SelectAccount.KnockWithMessage(item);

        SelectedIndex = AccountsList.IndexOf(item);
    }

    public async void AddAccount()
    {
        await DialogHost.Show(new AddAccountDialog());
    }

    #endregion
}