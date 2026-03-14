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
        return upstream.Subscribe(BuildRateLimited(subscriberFunc), bufferSize, overflowBehavior);
    }

    public IDisposable Subscribe(Action<T> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(
            BuildRateLimited(value => { subscriberFunc(value); return Task.CompletedTask; }),
            bufferSize,
            overflowBehavior);
    }

    private Func<T, Task> BuildRateLimited(Func<T, Task> invoke)
    {
        var lockObj = new object();
        var queue = new Queue<T>();
        Timer? timer = null;

        void Tick(object? sender, ElapsedEventArgs e)
        {
            T value;
            lock (lockObj)
            {
                if (queue.Count == 0)
                {
                    timer?.Dispose();
                    timer = null;
                    return;
                }
                value = queue.Dequeue();
                if (queue.Count == 0)
                {
                    timer?.Dispose();
                    timer = null;
                }
            }
            _ = invoke(value);
        }

        return value =>
        {
            lock (lockObj)
            {
                if (maxQueueSize > 0 && queue.Count >= maxQueueSize)
                {
                    if (queueOverflowBehavior == BufferOverflowBehavior.DropNewest)
                        return Task.CompletedTask;

                    // DropOldest: remove the front of the queue to make room
                    queue.Dequeue();
                }

                queue.Enqueue(value);
                if (timer == null)
                {
                    // Dispatch the first value immediately, start the interval timer for the rest
                    T first = queue.Dequeue();
                    if (queue.Count > 0)
                    {
                        timer = new Timer(intervalMillis) { AutoReset = true };
                        timer.Elapsed += Tick;
                        timer.Start();
                    }
                    _ = invoke(first);
                }
            }
            return Task.CompletedTask;
        };
    }
}
