using Avalonia.Controls;
using YetAnotherMinecraftLauncher.ViewModels;

namespace YetAnotherMinecraftLauncher.Views;

public partial class VersionsView : UserControl
{
    public VersionsView()
    {
        InitializeComponent();

        DataContext = new VersionsViewModel();
    }
}