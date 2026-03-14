using System.Timers;
using Timer = System.Timers.Timer;

namespace Ocluse.LiquidSnow.Data.DerivedDataFlows;

internal sealed class DebouncedDataFlow<T>(IDataFlow<T> upstream, int delayMillis, int maxWaitMillis) : IDataFlow<T>
{
    public bool Paused => upstream.Paused;

    public IDisposable Subscribe(Func<T, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(BuildDebounced(subscriberFunc), bufferSize, overflowBehavior);
    }

    public IDisposable Subscribe(Action<T> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(
            BuildDebounced(value => { subscriberFunc(value); return Task.CompletedTask; }),
            bufferSize,
            overflowBehavior);
    }

    private Func<T, Task> BuildDebounced(Func<T, Task> invoke)
    {
        var lockObj = new object();
        T pendingValue = default!;
        Timer? delayTimer = null;
        Timer? maxWaitTimer = null;

        void Flush(object? sender, ElapsedEventArgs e)
        {
            T value;
            lock (lockObj)
            {
                value = pendingValue;
                pendingValue = default!;
                delayTimer?.Dispose();
                delayTimer = null;
                maxWaitTimer?.Dispose();
                maxWaitTimer = null;
            }
            _ = invoke(value);
        }

        return value =>
        {
            lock (lockObj)
            {
                pendingValue = value;

                if (delayTimer != null)
                {
                    delayTimer.Stop();
                    delayTimer.Start();
                }
                else
                {
                    delayTimer = new Timer(delayMillis) { AutoReset = false };
                    delayTimer.Elapsed += Flush;
                    delayTimer.Start();
                }

                if (maxWaitMillis > 0 && maxWaitTimer == null)
                {
                    maxWaitTimer = new Timer(maxWaitMillis) { AutoReset = false };
                    maxWaitTimer.Elapsed += Flush;
                    maxWaitTimer.Start();
                }
            }

            return Task.CompletedTask;
        };
    }
}
