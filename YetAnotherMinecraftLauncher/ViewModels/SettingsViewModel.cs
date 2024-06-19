using System.Reactive;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Views;

namespace YetAnotherMinecraftLauncher.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> ReturnActionCommand { get; set; }

        #region Logic

        //ReturnActionCommand
        public void ReturnToHome()
        {
            MessageBus.Current.SendMessage(nameof(ReturnToHome));
        }

        #endregion

        public SettingsViewModel()
        {
            #region Register commands

            ReturnActionCommand = ReactiveCommand.Create(ReturnToHome);

            #endregion
        }
    }
}
