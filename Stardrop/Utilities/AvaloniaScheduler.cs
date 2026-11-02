using Avalonia.Threading;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Stardrop.Utilities
{
    /// <summary>
    /// ReactiveUI scheduler that runs work on Avalonia's UI thread. Avalonia 12 dropped the
    /// Avalonia.ReactiveUI package (and with it AvaloniaScheduler), so a small replacement lives here.
    /// </summary>
    public sealed class AvaloniaScheduler : LocalScheduler
    {
        public static readonly AvaloniaScheduler Instance = new AvaloniaScheduler();

        private AvaloniaScheduler()
        {
        }

        public override IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            var subscription = new SingleAssignmentDisposable();

            Dispatcher.UIThread.Post(() =>
            {
                if (subscription.IsDisposed is false)
                {
                    subscription.Disposable = action(this, state);
                }
            });

            return subscription;
        }

        public override IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var subscription = new SingleAssignmentDisposable();
            var timer = new DispatcherTimer { Interval = dueTime < TimeSpan.Zero ? TimeSpan.Zero : dueTime };

            timer.Tick += (_, _) =>
            {
                timer.Stop();

                if (subscription.IsDisposed is false)
                {
                    subscription.Disposable = action(this, state);
                }
            };

            timer.Start();

            return new CompositeDisposable(subscription, Disposable.Create(timer.Stop));
        }
    }
}
