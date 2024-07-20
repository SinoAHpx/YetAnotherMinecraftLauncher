using Avalonia.Controls;
using YetAnotherMinecraftLauncher.ViewModels.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher.Views.Controls.Dialogs
{
    public partial class DownloadingDialog : UserControl
    {
        public DownloadingDialog()
        {
            InitializeComponent();

            DataContext = new DownloadingDialogViewModel();
        }
    }
}
