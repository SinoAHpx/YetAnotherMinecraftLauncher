 using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using DialogHostAvalonia;
using DynamicData;
using Manganese.Array;
using Manganese.Text;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Models.Data;
using YetAnotherMinecraftLauncher.Models.Messages;
using YetAnotherMinecraftLauncher.Utils;
using YetAnotherMinecraftLauncher.Views;
using YetAnotherMinecraftLauncher.Views.Controls;
using YetAnotherMinecraftLauncher.Views.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher.ViewModels
{
    internal class AccountsViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> ReturnCommand { get; set; }

        private ObservableCollection<SelectiveItem> _accountsList = [];

        public ObservableCollection<SelectiveItem> AccountsList
        {
            get => _accountsList;
            set => this.RaiseAndSetIfChanged(ref _accountsList, value);
        }

        public ReactiveCommand<Unit, Unit> AddAccountCommand { get; set; }

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
                //todo: concrete logic is not finished
                MessengerRoutes.RemoveAccount.KnockWithMessage(item);
            }

        }

        public void SelectAccount(SelectiveItem item)
        {
            MessengerRoutes.SelectAccount.KnockWithMessage(item);
        }

        public async void AddAccount()
        {
            await DialogHost.Show(new AddAccountDialog());
        }

        #endregion

        public AccountsViewModel()
        {
            #region Register commands

            ReturnCommand = ReactiveCommand.Create(ReturnToHome);
            AddAccountCommand = ReactiveCommand.Create(AddAccount);

            #endregion
            UpdateAccounts();

            MessengerRoutes.UpdateAccounts.Subscribe<string>(_ =>
            {
                UpdateAccounts();
            });
        }

        private async void UpdateAccounts()
        {
            var accounts = await AccountUtils.ReadAsync();
            if (accounts.Count == 0)
            {
                return;
            }

            AccountsList.Clear();
            foreach (var account in accounts)
            {
                var item = new SelectiveItem
                {
                    Avatar = DefaultAssets.AccountAvatar,
                    Title = account.Name,
                    Subtitle = account.RefreshToken.IsNullOrEmpty() ? "Offline" : "Microsoft"
                };
                item.SelectAction = ReactiveCommand.Create(() =>
                {
                    SelectAccount(item);
                });
                item.RemoveAction = ReactiveCommand.Create(() =>
                {
                    RemoveAccount(item);
                });

                AccountsList.Add(item);
            }
        }
    }
}
