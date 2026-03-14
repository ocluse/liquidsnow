using System.Timers;
using Timer = System.Timers.Timer;

namespace Ocluse.LiquidSnow.Data.DerivedDataFlows;

internal sealed class ThrottledDataFlow<T>(IDataFlow<T> upstream, int delayMillis, bool trailing) : IDataFlow<T>
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

    private ThrottledSubscription BuildSubscription(Func<T, Task> invoke, int bufferSize, BufferOverflowBehavior overflowBehavior)
    {
        var state = new ThrottledState(invoke, delayMillis, trailing);
        var inner = upstream.Subscribe(state.OnValue, bufferSize, overflowBehavior);
        return new ThrottledSubscription(inner, state);
    }

    private sealed class ThrottledState(Func<T, Task> invoke, int delayMillis, bool trailing)
    {
        private readonly object _lock = new();
        private T _pendingValue = default!;
        private bool _pendingValueSet;
        private bool _disposed;
        private Timer? _delayTimer;

        public Task OnValue(T value)
        {
            lock (_lock)
            {
                if (_disposed) return Task.CompletedTask;

                if (_delayTimer != null)
                {
                    if (trailing)
                    {
                        _pendingValue = value;
                        _pendingValueSet = true;
                    }
                    return Task.CompletedTask;
                }

                _delayTimer = new Timer(delayMillis) { AutoReset = false };
                _delayTimer.Elapsed += Flush;
                _delayTimer.Start();
            }

            return invoke(value);
        }

        private void Flush(object? sender, ElapsedEventArgs e)
        {
            T value;
            bool shouldInvoke;
            lock (_lock)
            {
                _delayTimer?.Dispose();
                _delayTimer = null;
                shouldInvoke = trailing && _pendingValueSet && !_disposed;
                value = _pendingValue;
                _pendingValue = default!;
                _pendingValueSet = false;
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
            }
        }
    }

    private sealed class ThrottledSubscription(IDisposable inner, ThrottledState state) : IDisposable
    {
        public void Dispose()
        {
            state.Dispose();
            inner.Dispose();
        }
    }
}
