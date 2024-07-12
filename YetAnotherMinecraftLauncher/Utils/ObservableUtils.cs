using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace YetAnotherMinecraftLauncher.Utils
{
    public static class ObservableUtils
    {
        public static IDisposable SubscribeOnUIThread<T>(this IObservable<T> observable, Action<T> onNext)
        {
            return observable.Subscribe(x =>
            {
                Dispatcher.UIThread.Invoke(() =>
                {
                    onNext(x);
                });
            });
        }
    }
}
