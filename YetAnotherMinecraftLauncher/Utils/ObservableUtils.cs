using System;
using Avalonia.Threading;

namespace YetAnotherMinecraftLauncher.Utils;

public static class ObservableUtils
{
    public static IDisposable SubscribeOnUIThread<T>(this IObservable<T> observable, Action<T> onNext)
    {
        return observable.Subscribe(x => { Dispatcher.UIThread.Invoke(() => { onNext(x); }); });
    }
}