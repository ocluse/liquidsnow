using System;
using System.Reactive.Linq;

namespace Ocluse.LiquidSnow.Venus.Blazor
{
    /// <summary>
    /// A class that provides debouncing functionality.
    /// </summary>
    public sealed class Debouncer : IDisposable
    {
        private IDisposable? _subscription;

        private readonly double _interval;

        private readonly Action _execute;

        private bool _disposed;
        
        /// <summary>
        /// Creates a new instance of <see cref="Debouncer"/>.
        /// </summary>
        /// <param name="interval">The interval in milliseconds after which the <paramref name="execute"/> action should be invoked</param>
        /// <param name="execute">The action to execute once the interval has elapsed</param>
        public Debouncer(double interval, Action execute)
        {
            _interval = interval;
            _execute = execute;
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Debouncer));
            }
        }

        /// <summary>
        /// Begins the debounce timer. Calling this method will cancel any previously pending debounce timer.
        /// </summary>
        public void Begin()
        {
            EnsureNotDisposed();
            _subscription?.Dispose();

            _subscription = Observable
                .Timer(TimeSpan.FromMilliseconds(_interval))
                .Subscribe((t)=>
                {
                    _execute.Invoke();
                });
        }

        /// <summary>
        /// Cancels the debounce timer, preventing the <see cref="Action"/> from being invoked.
        /// </summary>
        public void Cancel()
        {
            EnsureNotDisposed();
            _subscription?.Dispose();
        }

        /// <summary>
        /// Disposes the <see cref="Debouncer"/> and cancels any pending debounce timer.
        /// </summary>
        public void Dispose()
        {
            EnsureNotDisposed();
            _subscription?.Dispose();
            _disposed = true;
        }
    }
}
