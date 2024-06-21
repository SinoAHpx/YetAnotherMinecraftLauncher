using Avalonia.Controls;

namespace YetAnotherMinecraftLauncher.Views.Controls.Dialogs
{
    public partial class AddAccountDialog : UserControl
    {
        public AddAccountDialog()
        {
            InitializeComponent();

            Grid.DataContext = this;
        }
    }
}
