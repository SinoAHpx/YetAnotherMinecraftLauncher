using Avalonia.Controls;
using YetAnotherMinecraftLauncher.ViewModels;

namespace YetAnotherMinecraftLauncher.Views
{
    public partial class AccountsView : UserControl
    {
        public AccountsView()
        {
            InitializeComponent();

            DataContext = new AccountsViewModel();
        }
    }
}
