using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using YetAnotherMinecraftLauncher.Views;

namespace YetAnotherMinecraftLauncher.Utils;

public static class LifetimeUtils
{
    public static Window GetMainWindow()
    {
        var mainWindow =
            (App.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime).MainWindow;

        return mainWindow;
    }
}