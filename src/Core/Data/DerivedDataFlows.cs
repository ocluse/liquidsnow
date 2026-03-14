using System.Timers;
using Timer = System.Timers.Timer;

namespace Ocluse.LiquidSnow.Data;

internal sealed class FilteredDataFlow<T>(IDataFlow<T> upstream, Func<T, bool> predicate) : IDataFlow<T>
{
    public bool Paused => upstream.Paused;

    public IDisposable Subscribe(Func<T, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(
            value => predicate(value) ? subscriberFunc(value) : Task.CompletedTask,
            bufferSize,
            overflowBehavior);
    }

    public IDisposable Subscribe(Action<T> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(
            value => { if (predicate(value)) subscriberFunc(value); },
            bufferSize,
            overflowBehavior);
    }

    public void Dispose()
    {
        // Derived flows do not own the upstream; disposal is a no-op.
        // Callers are responsible for disposing the upstream flow directly.
    }
}

internal sealed class TransformedDataFlow<T, TResult>(IDataFlow<T> upstream, Func<T, TResult> selector) : IDataFlow<TResult>
{
    public bool Paused => upstream.Paused;

    public IDisposable Subscribe(Func<TResult, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(
            value => subscriberFunc(selector(value)),
            bufferSize,
            overflowBehavior);
    }

    public IDisposable Subscribe(Action<TResult> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(
            value => subscriberFunc(selector(value)),
            bufferSize,
            overflowBehavior);
    }

    public void Dispose()
    {
        // Derived flows do not own the upstream; disposal is a no-op.
        // Callers are responsible for disposing the upstream flow directly.
    }
}

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

    public void Dispose()
    {
        // Derived flows do not own the upstream; disposal is a no-op.
        // Callers are responsible for disposing the upstream flow directly.
    }
}

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

    public void Dispose()
    {
        // Derived flows do not own the upstream; disposal is a no-op.
        // Callers are responsible for disposing the upstream flow directly.
    }
}

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

    public void Dispose()
    {
        // Derived flows do not own the upstream; disposal is a no-op.
        // Callers are responsible for disposing the upstream flow directly.
    }
}

internal sealed class BatchedDataFlow<T>(
    IDataFlow<T> upstream,
    int maxSize,
    int windowMillis,
    BatchFlushBehavior flushBehavior) : IDataFlow<IReadOnlyList<T>>
{
    // maxSize == 0  → window-only mode
    // windowMillis == 0 → count-only mode

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

    private IDisposable BuildSubscription(Func<IReadOnlyList<T>, Task> invoke, int bufferSize, BufferOverflowBehavior overflowBehavior)
    {
        var lockObj = new object();
        var buffer = new List<T>();
        Timer? timer = null;
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
                if (buffer.Count == 0 && !windowOnly)
                    return;
                batch = Snapshot();
            }
            _ = invoke(batch);
        }

        var innerSubscription = upstream.Subscribe(value =>
        {
            IReadOnlyList<T>? toFlush = null;
            lock (lockObj)
            {
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

        return new BatchSubscription(innerSubscription, lockObj, buffer, () =>
        {
            timer?.Dispose();
            timer = null;
        }, flushBehavior, invoke);
    }

    public void Dispose()
    {
        // Derived flows do not own the upstream; disposal is a no-op.
        // Callers are responsible for disposing the upstream flow directly.
    }

    private sealed class BatchSubscription(
        IDisposable inner,
        object lockObj,
        List<T> buffer,
        Action stopTimer,
        BatchFlushBehavior flushBehavior,
        Func<IReadOnlyList<T>, Task> invoke) : IDisposable
    {
        public void Dispose()
        {
            IReadOnlyList<T>? toFlush = null;
            lock (lockObj)
            {
                stopTimer();
                if (flushBehavior == BatchFlushBehavior.Flush && buffer.Count > 0)
                {
                    toFlush = buffer.ToList();
                    buffer.Clear();
                }
            }
            if (toFlush != null)
                _ = invoke(toFlush);
            inner.Dispose();
        }
    }
}

internal sealed class DoDataFlow<T>(IDataFlow<T> upstream, Action<T> sideEffect) : IDataFlow<T>
{
    public bool Paused => upstream.Paused;

    public IDisposable Subscribe(Func<T, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(value =>
        {
            sideEffect(value);
            return subscriberFunc(value);
        }, bufferSize, overflowBehavior);
    }

    public IDisposable Subscribe(Action<T> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(value =>
        {
            sideEffect(value);
            subscriberFunc(value);
        }, bufferSize, overflowBehavior);
    }

    public void Dispose()
    {
        // Derived flows do not own the upstream; disposal is a no-op.
        // Callers are responsible for disposing the upstream flow directly.
    }
}

internal sealed class DistinctUntilChangedDataFlow<T>(IDataFlow<T> upstream, IEqualityComparer<T> comparer) : IDataFlow<T>
{
    public bool Paused => upstream.Paused;

    public IDisposable Subscribe(Func<T, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(BuildDistinct(subscriberFunc), bufferSize, overflowBehavior);
    }

    public IDisposable Subscribe(Action<T> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(
            BuildDistinct(value => { subscriberFunc(value); return Task.CompletedTask; }),
            bufferSize,
            overflowBehavior);
    }

    private Func<T, Task> BuildDistinct(Func<T, Task> invoke)
    {
        T lastValue = default!;
        bool hasValue = false;

        return value =>
        {
            if (hasValue && comparer.Equals(value, lastValue))
                return Task.CompletedTask;

            lastValue = value;
            hasValue = true;
            return invoke(value);
        };
    }

    public void Dispose()
    {
        // Derived flows do not own the upstream; disposal is a no-op.
        // Callers are responsible for disposing the upstream flow directly.
    }
}

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

    private IDisposable BuildSubscription(Func<T, Task> invoke, int bufferSize, BufferOverflowBehavior overflowBehavior)
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

    public void Dispose()
    {
        // Derived flows do not own the upstream; disposal is a no-op.
        // Callers are responsible for disposing the upstream flow directly.
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
