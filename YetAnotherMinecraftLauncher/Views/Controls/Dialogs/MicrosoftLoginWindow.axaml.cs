using Avalonia.Controls;
using YetAnotherMinecraftLauncher.ViewModels.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher.Views.Controls.Dialogs
{
    public partial class MicrosoftLoginWindow : Window
    {
        public MicrosoftLoginWindow(string loginUrl)
        {
            InitializeComponent();

            DataContext = new MicrosoftLoginWindowViewModel(loginUrl);
        }
    }
}
