using ReactiveUI;
using System.Reactive;

namespace YetAnotherMinecraftLauncher.ViewModels;

public class VersionsViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> ReturnActionCommand { get; set; }

    #region Logic

    //ReturnActionCommand
    public void ReturnToHome()
    {
        MessageBus.Current.SendMessage(nameof(ReturnToHome));
    }

    #endregion

    public VersionsViewModel()
    {
        #region Register commands

        ReturnActionCommand = ReactiveCommand.Create(ReturnToHome);

        #endregion
    }
}