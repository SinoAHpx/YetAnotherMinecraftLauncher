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
            MessageBus.Current.SendMessage(nameof(ReturnToHome));
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
                MessageBus.Current.SendMessage(authResult, "Accounts");
            });
            //todo: refactor this
            authResult.RemoveAction = ReactiveCommand.Create(async () =>
            {
                var dialog = new ConfirmDialog
                {
                    Message = "Deletion cannot be undone",
                    CancelActionCommand = ReactiveCommand.Create(() =>
                    {
                        DialogHost.Close(null);
                    }),
                    ConfirmActionCommand = ReactiveCommand.Create(() =>
                    {
                        AccountsList.Remove(authResult);
                        DialogHost.Close(null);
                    })
                };
                await DialogHost.Show(dialog);
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
