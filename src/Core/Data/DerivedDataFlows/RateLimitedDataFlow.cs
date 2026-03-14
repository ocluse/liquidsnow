using System.Timers;
using Timer = System.Timers.Timer;

namespace Ocluse.LiquidSnow.Data.DerivedDataFlows;

internal sealed class RateLimitedDataFlow<T>(
    IDataFlow<T> upstream,
    int intervalMillis,
    int maxQueueSize,
    BufferOverflowBehavior queueOverflowBehavior) : IDataFlow<T>
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

    private RateLimitedSubscription BuildSubscription(Func<T, Task> invoke, int bufferSize, BufferOverflowBehavior overflowBehavior)
    {
        var state = new RateLimitedState(invoke, intervalMillis, maxQueueSize, queueOverflowBehavior);
        var inner = upstream.Subscribe(state.OnValue, bufferSize, overflowBehavior);
        return new RateLimitedSubscription(inner, state);
    }

    private sealed class RateLimitedState(
        Func<T, Task> invoke,
        int intervalMillis,
        int maxQueueSize,
        BufferOverflowBehavior queueOverflowBehavior)
    {
        private readonly object _lock = new();
        private readonly Queue<T> _queue = new();
        private bool _disposed;
        private Timer? _timer;

        public Task OnValue(T value)
        {
            lock (_lock)
            {
                if (_disposed) return Task.CompletedTask;

                if (maxQueueSize > 0 && _queue.Count >= maxQueueSize)
                {
                    if (queueOverflowBehavior == BufferOverflowBehavior.DropNewest)
                        return Task.CompletedTask;

                    // DropOldest: remove the front of the queue to make room
                    _queue.Dequeue();
                }

                _queue.Enqueue(value);

                if (_timer == null)
                {
                    // Dispatch the first value immediately; start the interval timer for the rest
                    T first = _queue.Dequeue();
                    if (_queue.Count > 0)
                    {
                        _timer = new Timer(intervalMillis) { AutoReset = true };
                        _timer.Elapsed += Tick;
                        _timer.Start();
                    }
                    _ = invoke(first);
                }
            }

            return Task.CompletedTask;
        }

        private void Tick(object? sender, ElapsedEventArgs e)
        {
            T value;
            lock (_lock)
            {
                if (_disposed || _queue.Count == 0)
                {
                    _timer?.Dispose();
                    _timer = null;
                    return;
                }
                value = _queue.Dequeue();
                if (_queue.Count == 0)
                {
                    _timer?.Dispose();
                    _timer = null;
                }
            }
            _ = invoke(value);
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _disposed = true;
                _timer?.Dispose();
                _timer = null;
                _queue.Clear();
            }
        }
    }

    private sealed class RateLimitedSubscription(IDisposable inner, RateLimitedState state) : IDisposable
    {
        public void Dispose()
        {
            state.Dispose();
            inner.Dispose();
        }
    }
}
