using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace YetAnotherMinecraftLauncher.Utils;

public static class LifetimeUtils
{
    public static Window GetMainWindow()
    {
        var mainWindow =
            (Application.Current.ApplicationLifetime as ClassicDesktopStyleApplicationLifetime).MainWindow;

        return mainWindow;
    }
}