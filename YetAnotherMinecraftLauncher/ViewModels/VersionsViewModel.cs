using ReactiveUI;
using System.Reactive;

namespace YetAnotherMinecraftLauncher.ViewModels;

public class VersionsViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> ReturnCommand { get; set; }

    #region Logic

    //ReturnCommand
    public void ReturnToHome()
    {
        MessageBus.Current.SendMessage(nameof(ReturnToHome));
    }

    #endregion

    public VersionsViewModel()
    {
        #region Register commands

        ReturnCommand = ReactiveCommand.Create(ReturnToHome);

        #endregion
    }
}