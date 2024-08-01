using System;
using System.Reactive;
using ReactiveUI;

namespace YetAnotherMinecraftLauncher.ViewModels.Controls.Dialogs;

internal class DownloadingDialogViewModel : ViewModelBase
{
    private ReactiveCommand<Unit, Unit> _cancelCommand;
    private int _currentProgress;


    private bool _isSingleFile;

    private int _totalProgress;

    public DownloadingDialogViewModel()
    {
        this.WhenAnyValue(x => x.TotalProgress).Subscribe(x => { IsSingleFile = x == 1; });
    }

    public int CurrentProgress
    {
        get => _currentProgress;
        set => this.RaiseAndSetIfChanged(ref _currentProgress, value);
    }

    public int TotalProgress
    {
        get => _totalProgress;
        set => this.RaiseAndSetIfChanged(ref _totalProgress, value);
    }

    public ReactiveCommand<Unit, Unit> CancelCommand
    {
        get => _cancelCommand;
        set => this.RaiseAndSetIfChanged(ref _cancelCommand, value);
    }

    public bool IsSingleFile
    {
        get => _isSingleFile;
        set => this.RaiseAndSetIfChanged(ref _isSingleFile, value);
    }
}