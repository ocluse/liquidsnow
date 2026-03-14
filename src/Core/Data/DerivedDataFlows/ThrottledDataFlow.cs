using System.Timers;
using Timer = System.Timers.Timer;

namespace Ocluse.LiquidSnow.Data.DerivedDataFlows;

internal sealed class ThrottledDataFlow<T>(IDataFlow<T> upstream, int delayMillis, bool trailing) : IDataFlow<T>
{
    public bool Paused => upstream.Paused;

    public IDisposable Subscribe(Func<T, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(BuildThrottled(subscriberFunc), bufferSize, overflowBehavior);
    }

    public IDisposable Subscribe(Action<T> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(
            BuildThrottled(value => { subscriberFunc(value); return Task.CompletedTask; }),
            bufferSize,
            overflowBehavior);
    }

    private Func<T, Task> BuildThrottled(Func<T, Task> invoke)
    {
        var lockObj = new object();
        T pendingValue = default!;
        bool pendingValueSet = false;
        Timer? delayTimer = null;

        void Flush(object? sender, ElapsedEventArgs e)
        {
            T value;
            bool shouldInvoke;
            lock (lockObj)
            {
                delayTimer?.Dispose();
                delayTimer = null;
                shouldInvoke = trailing && pendingValueSet;
                value = pendingValue;
                pendingValue = default!;
                pendingValueSet = false;
            }
            if (shouldInvoke) _ = invoke(value);
        }

        return value =>
        {
            lock (lockObj)
            {
                if (delayTimer != null)
                {
                    if (trailing)
                    {
                        pendingValue = value;
                        pendingValueSet = true;
                    }
                    return Task.CompletedTask;
                }

                delayTimer = new Timer(delayMillis) { AutoReset = false };
                delayTimer.Elapsed += Flush;
                delayTimer.Start();
            }

            return invoke(value);
        };
    }
}
