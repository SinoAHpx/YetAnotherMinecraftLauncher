using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace YetAnotherMinecraftLauncher.ViewModels.Controls.Dialogs
{
    internal class DownloadingDialogViewModel : ViewModelBase
    {
        private int _currentProgress;

        public int CurrentProgress
        {
            get => _currentProgress;
            set => this.RaiseAndSetIfChanged(ref _currentProgress, value);
        }

        private int _totalProgress;

        public int TotalProgress
        {
            get => _totalProgress;
            set => this.RaiseAndSetIfChanged(ref _totalProgress, value);
        }


        private ReactiveCommand<Unit, Unit> _cancelCommand;

        public ReactiveCommand<Unit, Unit> CancelCommand
        {
            get => _cancelCommand;
            set => this.RaiseAndSetIfChanged(ref _cancelCommand, value);
        }
    }
}
