using Avalonia.Controls;
using YetAnotherMinecraftLauncher.ViewModels.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher.Views.Controls.Dialogs
{
    public partial class AddAccountDialog : UserControl
    {
        public AddAccountDialog()
        {
            InitializeComponent();

            DataContext = new AddAccountDialogViewModel();
        }
    }
}
