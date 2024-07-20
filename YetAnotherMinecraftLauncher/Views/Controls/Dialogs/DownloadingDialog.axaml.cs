using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using DialogHostAvalonia;
using ReactiveUI;
using Xilium.CefGlue;
using YetAnotherMinecraftLauncher.ViewModels.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher.Views.Controls.Dialogs
{
    public partial class DownloadingDialog : UserControl
    {
        public DownloadingDialog()
        {
            InitializeComponent();

            DataContext = _viewModel;
        }

        private readonly DownloadingDialogViewModel _viewModel = new();


        public async Task<DownloadingDialog> ShowDialogAsync(int totalProgress, Action cancelAction)
        {
            _viewModel.TotalProgress = totalProgress;
            _viewModel.CurrentProgress = 0;
            _viewModel.CancelCommand = ReactiveCommand.Create(cancelAction);
            await DialogHost.Show(this);
            
            return this;
        }

        public void Update(int currentProgress)
        {
            _viewModel.CurrentProgress = currentProgress;
        }
    }
}
