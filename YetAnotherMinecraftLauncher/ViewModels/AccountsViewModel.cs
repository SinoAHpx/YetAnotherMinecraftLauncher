using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Views;

namespace YetAnotherMinecraftLauncher.ViewModels
{
    internal class AccountsViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> ReturnActionCommand { get; set; }

        #region Actual logics

        //ReturnActionCommand
        public void ReturnToHome()
        {
            MessageBus.Current.SendMessage(nameof(ReturnToHome), nameof(AccountsView));
        }

        #endregion

        public AccountsViewModel()
        {
            ReturnActionCommand = ReactiveCommand.Create(ReturnToHome);
        }
    }
}
