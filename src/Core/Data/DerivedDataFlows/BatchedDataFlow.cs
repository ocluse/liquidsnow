using System.Timers;
using Timer = System.Timers.Timer;

namespace Ocluse.LiquidSnow.Data.DerivedDataFlows;

internal sealed class BatchedDataFlow<T>(
    IDataFlow<T> upstream,
    int maxSize,
    int windowMillis,
    BatchFlushBehavior flushBehavior) : IDataFlow<IReadOnlyList<T>>
{
    // maxSize == 0  -> window-only mode
    // windowMillis == 0 -> count-only mode

    public bool Paused => upstream.Paused;

    public IDisposable Subscribe(Func<IReadOnlyList<T>, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return BuildSubscription(subscriberFunc, bufferSize, overflowBehavior);
    }

    public IDisposable Subscribe(Action<IReadOnlyList<T>> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return BuildSubscription(
            batch => { subscriberFunc(batch); return Task.CompletedTask; },
            bufferSize,
            overflowBehavior);
    }

    private BatchSubscription BuildSubscription(Func<IReadOnlyList<T>, Task> invoke, int bufferSize, BufferOverflowBehavior overflowBehavior)
    {
        var lockObj = new object();
        var buffer = new List<T>();
        Timer? timer = null;
        var disposed = new DisposedFlag();
        bool windowOnly = maxSize == 0;

        IReadOnlyList<T> Snapshot()
        {
            var snapshot = buffer.ToList();
            buffer.Clear();
            return snapshot;
        }

        void OnTick(object? sender, ElapsedEventArgs e)
        {
            IReadOnlyList<T> batch;
            lock (lockObj)
            {
                if (disposed.Value) return;
                if (buffer.Count == 0 && !windowOnly) return;
                batch = Snapshot();
            }
            _ = invoke(batch);
        }

        var innerSubscription = upstream.Subscribe(value =>
        {
            IReadOnlyList<T>? toFlush = null;
            lock (lockObj)
            {
                if (disposed.Value) return Task.CompletedTask;

                buffer.Add(value);

                // Start window timer on first item in the window
                if (windowMillis > 0 && timer == null)
                {
                    timer = new Timer(windowMillis) { AutoReset = true };
                    timer.Elapsed += OnTick;
                    timer.Start();
                }

                if (maxSize > 0 && buffer.Count >= maxSize)
                {
                    toFlush = Snapshot();
                    // In count-only mode stop the timer since the buffer is now empty
                    if (windowMillis == 0)
                    {
                        timer?.Dispose();
                        timer = null;
                    }
                }
            }
            if (toFlush != null)
                _ = invoke(toFlush);
            return Task.CompletedTask;
        }, bufferSize, overflowBehavior);

        return new BatchSubscription(innerSubscription, lockObj, buffer, disposed, () =>
        {
            timer?.Dispose();
            timer = null;
        }, flushBehavior, invoke);
    }

    // Simple mutable wrapper so nested classes/lambdas can share the disposed flag.
    private sealed class DisposedFlag { public bool Value; }

    private sealed class BatchSubscription(
        IDisposable inner,
        object lockObj,
        List<T> buffer,
        DisposedFlag disposed,
        Action stopTimer,
        BatchFlushBehavior flushBehavior,
        Func<IReadOnlyList<T>, Task> invoke) : IDisposable
    {
        public void Dispose()
        {
            IReadOnlyList<T>? toFlush = null;
            lock (lockObj)
            {
                disposed.Value = true;
                stopTimer();
                if (flushBehavior == BatchFlushBehavior.Flush && buffer.Count > 0)
                {
                    toFlush = [.. buffer];
                    buffer.Clear();
                }
            }
            if (toFlush != null)
                _ = invoke(toFlush);
            inner.Dispose();
        }
    }
}
