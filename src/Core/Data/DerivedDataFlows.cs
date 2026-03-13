using System.Collections.Concurrent;
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

internal sealed class RateLimitedDataFlow<T>(IDataFlow<T> upstream, int intervalMillis) : IDataFlow<T>
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
