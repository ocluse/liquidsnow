using System.Timers;
using Timer = System.Timers.Timer;

namespace Ocluse.LiquidSnow.Data.DerivedDataFlows;

internal sealed class DebouncedDataFlow<T>(IDataFlow<T> upstream, int delayMillis, int maxWaitMillis) : IDataFlow<T>
{
    public bool Paused => upstream.Paused;

    public IDisposable Subscribe(Func<T, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return BuildSubscription(subscriberFunc, bufferSize, overflowBehavior);
    }

    public IDisposable Subscribe(Action<T> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return BuildSubscription(
            value => { subscriberFunc(value); return Task.CompletedTask; },
            bufferSize,
            overflowBehavior);
    }

    private DebouncedSubscription BuildSubscription(Func<T, Task> invoke, int bufferSize, BufferOverflowBehavior overflowBehavior)
    {
        var state = new DebouncedState(invoke, delayMillis, maxWaitMillis);

        var inner = upstream.Subscribe(state.OnValue, bufferSize, overflowBehavior);

        return new DebouncedSubscription(inner, state);
    }

    private sealed class DebouncedState(Func<T, Task> invoke, int delayMillis, int maxWaitMillis)
    {
        private readonly object _lock = new();
        private T _pendingValue = default!;
        private bool _hasPending;
        private bool _disposed;
        private Timer? _delayTimer;
        private Timer? _maxWaitTimer;

        public Task OnValue(T value)
        {
            lock (_lock)
            {
                if (_disposed) return Task.CompletedTask;

                _pendingValue = value;
                _hasPending = true;

                if (_delayTimer != null)
                {
                    _delayTimer.Stop();
                    _delayTimer.Start();
                }
                else
                {
                    _delayTimer = new Timer(delayMillis) { AutoReset = false };
                    _delayTimer.Elapsed += Flush;
                    _delayTimer.Start();
                }

                if (maxWaitMillis > 0 && _maxWaitTimer == null)
                {
                    _maxWaitTimer = new Timer(maxWaitMillis) { AutoReset = false };
                    _maxWaitTimer.Elapsed += Flush;
                    _maxWaitTimer.Start();
                }
            }

            return Task.CompletedTask;
        }

        private void Flush(object? sender, ElapsedEventArgs e)
        {
            T value;
            bool shouldInvoke;
            lock (_lock)
            {
                shouldInvoke = _hasPending && !_disposed;
                value = _pendingValue;
                _pendingValue = default!;
                _hasPending = false;
                _delayTimer?.Dispose();
                _delayTimer = null;
                _maxWaitTimer?.Dispose();
                _maxWaitTimer = null;
            }
            if (shouldInvoke) _ = invoke(value);
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _disposed = true;
                _delayTimer?.Dispose();
                _delayTimer = null;
                _maxWaitTimer?.Dispose();
                _maxWaitTimer = null;
            }
        }
    }

    private sealed class DebouncedSubscription(IDisposable inner, DebouncedState state) : IDisposable
    {
        public void Dispose()
        {
            state.Dispose();
            inner.Dispose();
        }
    }
}
