using Avalonia.Controls;
using YetAnotherMinecraftLauncher.ViewModels;

namespace YetAnotherMinecraftLauncher.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();

        DataContext = new SettingsViewModel();
    }
}