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
using ReactiveUI;
using YetAnotherMinecraftLauncher.Models.Messages;
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
            var accountAddingDialog = new AddAccountDialog();

            if (await DialogHost.Show(accountAddingDialog) is not SelectiveItem authResult)
            {
                return;
            }
            
            authResult.SelectAction = ReactiveCommand.Create(() =>
            {
                SelectAccount(authResult);
            });
            authResult.RemoveAction = ReactiveCommand.Create( () =>
            {
                RemoveAccount(authResult);
            });
            AccountsList.Add(authResult);
        }

        #endregion

        public AccountsViewModel()
        {
            #region Register commands

            ReturnCommand = ReactiveCommand.Create(ReturnToHome);
            AddAccountCommand = ReactiveCommand.Create(AddAccount);

            #endregion
        }
    }
}
