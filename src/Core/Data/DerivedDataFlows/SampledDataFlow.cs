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
        var lockObj = new object();
        T lastValue = default!;
        bool hasValue = false;
        bool hasFresh = false;

        var timer = new Timer(intervalMillis) { AutoReset = true };

        timer.Elapsed += (_, _) =>
        {
            T value;
            lock (lockObj)
            {
                if (!hasValue)
                    return;

                if (!hasFresh && emptyBehavior == SampleEmptyBehavior.Skip)
                    return;

                value = lastValue;
                hasFresh = false;
            }
            _ = invoke(value);
        };

        var innerSubscription = upstream.Subscribe(value =>
        {
            lock (lockObj)
            {
                lastValue = value;
                hasValue = true;
                hasFresh = true;
            }
            return Task.CompletedTask;
        }, bufferSize, overflowBehavior);

        timer.Start();

        return new SampleSubscription(innerSubscription, timer);
    }

    private sealed class SampleSubscription(IDisposable inner, Timer timer) : IDisposable
    {
        public void Dispose()
        {
            timer.Dispose();
            inner.Dispose();
        }
    }
}
