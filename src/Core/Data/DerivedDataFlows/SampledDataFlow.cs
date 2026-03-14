using Timer = System.Timers.Timer;

namespace Ocluse.LiquidSnow.Data.DerivedDataFlows;

internal sealed class SampledDataFlow<T>(IDataFlow<T> upstream, int intervalMillis, SampleEmptyBehavior emptyBehavior) : IDataFlow<T>
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

    private SampleSubscription BuildSubscription(Func<T, Task> invoke, int bufferSize, BufferOverflowBehavior overflowBehavior)
    {
        var state = new SampledState(invoke, emptyBehavior);

        var innerSubscription = upstream.Subscribe(state.OnValue, bufferSize, overflowBehavior);

        var timer = new Timer(intervalMillis) { AutoReset = true };
        timer.Elapsed += state.OnTick;
        timer.Start();

        return new SampleSubscription(innerSubscription, timer, state);
    }

    private sealed class SampledState(Func<T, Task> invoke, SampleEmptyBehavior emptyBehavior)
    {
        private readonly object _lock = new();
        private T _lastValue = default!;
        private bool _hasValue;
        private bool _hasFresh;
        private bool _disposed;

        public Task OnValue(T value)
        {
            lock (_lock)
            {
                _lastValue = value;
                _hasValue = true;
                _hasFresh = true;
            }
            return Task.CompletedTask;
        }

        public void OnTick(object? sender, System.Timers.ElapsedEventArgs e)
        {
            T value;
            lock (_lock)
            {
                if (_disposed || !_hasValue) return;
                if (!_hasFresh && emptyBehavior == SampleEmptyBehavior.Skip) return;
                value = _lastValue;
                _hasFresh = false;
            }
            _ = invoke(value);
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _disposed = true;
            }
        }
    }

    private sealed class SampleSubscription(IDisposable inner, Timer timer, SampledState state) : IDisposable
    {
        public void Dispose()
        {
            state.Dispose();
            timer.Dispose();
            inner.Dispose();
        }
    }
}
