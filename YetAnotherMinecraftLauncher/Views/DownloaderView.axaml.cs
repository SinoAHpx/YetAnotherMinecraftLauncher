using Avalonia.Controls;
using YetAnotherMinecraftLauncher.ViewModels;

namespace YetAnotherMinecraftLauncher.Views;

public partial class DownloaderView : UserControl
{
    public DownloaderView()
    {
        InitializeComponent();

        DataContext = new DownloaderViewModel();
    }
}