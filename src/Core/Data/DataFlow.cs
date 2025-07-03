using System.Collections.Concurrent;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// A class that provides methods for creating and managing data flows.
/// </summary>
public static class DataFlow
{
    /// <summary>
    /// Creates a new data flow with the specified parameters.
    /// </summary>
    /// <param name="replayCount">The number of values stored as history that will be replayed to new subscribers.</param>
    /// <param name="delayMillis">The delay of a throttled or debounced flow.</param>
    /// <param name="maxWaitMillis">Used by a debounced flow to ensure that a value is dispatched at least once within a given frame determined by the value.</param>
    /// <param name="mode">Determines the mode of operation that the flow will use.</param>
    /// <returns>A data flow with configured with the specified parameters.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IDataFlow<T> Create<T>(
        int replayCount,
        int delayMillis,
        int maxWaitMillis,
        DataFlowMode mode)
    {
        if (replayCount < 0) throw new ArgumentOutOfRangeException(nameof(replayCount), "Replay count must be non-negative.");
        if (delayMillis < 0) throw new ArgumentOutOfRangeException(nameof(delayMillis), "Delay must be non-negative.");
        if (maxWaitMillis < 0) throw new ArgumentOutOfRangeException(nameof(maxWaitMillis), "Max wait time must be non-negative.");
        return new DataFlow<T>(replayCount, delayMillis, maxWaitMillis, mode);
    }

    /// <summary>
    /// Creates a data flow that emits it values normally (as soon as they arrive).
    /// </summary>
    /// <param name="replayCount">The number of values stored as history that will be replayed to new subscribers.</param>
    /// <returns></returns>
    public static IDataFlow<T> Create<T>(
        int replayCount = 0)
    {
        return Create<T>(replayCount, 0, 0, DataFlowMode.Normal);
    }

    /// <summary>
    /// Creates a data flow that will debounce values emitted to it.
    /// </summary>
    /// <param name="delayMillis">The delay before the latest emitted value is dispatched within a given frame.</param>
    /// <param name="maxWaitMillis">The maximum time to wait to ensure that a value is dispatched at least once within a given debounce frame</param>
    /// <param name="replayCount">The number of values stored as history that will be replayed to new subscribers.</param>
    /// <returns></returns>
    public static IDataFlow<T> CreateDebounce<T>(
        int delayMillis,
        int maxWaitMillis = 0,
        int replayCount = 0)
    {
        return Create<T>(replayCount, delayMillis, maxWaitMillis, DataFlowMode.Debounce);
    }

    /// <summary>
    /// Creates a data flow that will throttle values emitted to it.
    /// </summary>
    /// <param name="delayMillis">How long a throttle should last, during which emitted values are dropped.</param>
    /// <param name="emitForTrailing">If true, if a value was set during the throttle window and the throttle window has ended, the last set value will be emitted.</param>
    /// <param name="replayCount">The number of values stored as history that will be replayed to new subscribers.</param>
    /// <returns></returns>
    public static IDataFlow<T> CreateThrottled<T>(
        int delayMillis,
        bool emitForTrailing,
        int replayCount = 0)
    {
        var mode = emitForTrailing ? DataFlowMode.ThrottleTrailing : DataFlowMode.Throttle;
        return Create<T>(replayCount, delayMillis, 0, mode);
    }
}

internal sealed class DataFlow<T>(
    int replayCount,
    int delayMillis,
    int maxWaitMillis,
    DataFlowMode mode) : IDataFlow<T> 
{
    private readonly HashSet<SubscriptionHandler> _handlers = [];
    private readonly List<T?> _history = [];
    private readonly object _lock = new();

    private bool _isPaused;
    private bool _pausedValueSet;
    private T? _pausedValue;

    private T? _pendingValue;
    private bool _pendingValueSet;

    private Timer? _delayTimer;
    private Timer? _maxWaitTimer;

    public bool Paused=> _isPaused;

    public DataFlowMode Mode => mode;

    public void Pause()
    {
        if (_isPaused)
        {
            throw new InvalidOperationException("Cannot pause a flow that is already paused.");
        }

        lock (_lock)
        {
            _isPaused = true;
        }
    }

    public void Resume()
    {
        if (!_isPaused)
        {
            throw new InvalidOperationException("Cannot resume a flow that is not paused.");
        }

        T? next;

        lock (_lock)
        {
            _isPaused = false;

            if (_pausedValueSet)
            {
                next = _pausedValue;
                _pausedValue = default;
                _pausedValueSet = false;
            }
            else
            {
                return;
            }
        }

        Emit(next);
    }

    public IDisposable Subscribe(
        Func<T?, Task> subscriber,
        int bufferSize,
        BufferOverflowBehavior overflowBehavior)
    {
        SubscriptionHandler handler = new(bufferSize, overflowBehavior, subscriber, null);
        AddHandler(handler);
        return handler;
    }

    public IDisposable Subscribe(Action<T?> subscriber, int bufferSize,
        BufferOverflowBehavior overflowBehavior)
    {
        SubscriptionHandler handler = new(bufferSize, overflowBehavior, null, subscriber);
        AddHandler(handler);
        return handler;
    }

    private void AddHandler(SubscriptionHandler handler)
    {
        lock (_lock)
        {
            if (replayCount > 0)
            {
                var start = Math.Max(0, _history.Count - replayCount);
                var recent = _history.Skip(start).ToList();
                handler.BufferInitial(recent);
            }
            _handlers.Add(handler);
        }
        handler.Start();
    }

    public void Emit(T? value)
    {
        lock (_lock)
        {
            if (_isPaused)
            {
                _pausedValueSet = true;
                _pausedValue = value;
                return;
            }

            switch (mode)
            {
                case DataFlowMode.Normal:
                    AppendHistory(value);
                    Dispatch(value);
                    return;
                case DataFlowMode.Debounce:
                    DebounceNotify(value);
                    return;
                case DataFlowMode.Throttle:
                case DataFlowMode.ThrottleTrailing:
                    ThrottleNotify(value);
                    return;
            }
        }
    }

    private void AppendHistory(T? value)
    {
        if (replayCount <= 0) return;
        _history.Add(value);
        if (_history.Count > replayCount)
            _history.RemoveAt(0);
    }

    private void Dispatch(T? value)
    {
        foreach (var handler in _handlers)
        {
            handler.Enqueue(value);
        }
    }

    private void DebounceNotify(T? value)
    {
        _pendingValue = value;
        if (_delayTimer != null)
        {
            _delayTimer.Stop();
            _delayTimer.Start();
        }
        else
        {
            _delayTimer = new Timer(delayMillis) { AutoReset = false };
            _delayTimer.Elapsed += FlushDebounce;
            _delayTimer.Start();
        }

        if (maxWaitMillis > 0 && _maxWaitTimer == null)
        {
            _maxWaitTimer = new(maxWaitMillis) { AutoReset = false };
            _maxWaitTimer.Elapsed += FlushDebounce;
            _maxWaitTimer.Start();
        }
    }

    private void ThrottleNotify(T? value)
    {
        if (_delayTimer != null)
        {
            // If already throttling, just update the pending value
            if (mode == DataFlowMode.ThrottleTrailing)
            {
                _pendingValue = value;
                _pendingValueSet = true;
            }

            return;
        }

        _delayTimer = new Timer(delayMillis) { AutoReset = false };
        _delayTimer.Elapsed += FlushThrottle;
        _delayTimer.Start();
    }

    private void FlushDebounce(object? sender, ElapsedEventArgs e)
    {
        T? value;
        lock (_lock)
        {
            value = _pendingValue;
            _pendingValue = default;
            _delayTimer?.Dispose();
            _delayTimer = null;
            _maxWaitTimer?.Dispose();
            _maxWaitTimer = null;

            AppendHistory(value);
            Dispatch(value);
        }
    }

    private void FlushThrottle(object? sender, ElapsedEventArgs e)
    {
        T? value;
        lock (_lock)
        {
            _delayTimer?.Dispose();
            _delayTimer = null;
            if (mode == DataFlowMode.ThrottleTrailing && _pendingValueSet)
            {
                value = _pendingValue;
                _pendingValue = default;
                _pendingValueSet = false;
            }
            else
            {
                return;
            }

            AppendHistory(value);
            Dispatch(value);
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            _delayTimer?.Dispose();
            _maxWaitTimer?.Dispose();
            _delayTimer = null;
            _maxWaitTimer = null;
            foreach (var handler in _handlers)
            {
                handler.Dispose();
            }
            _handlers.Clear();
        }
    }

    private sealed class SubscriptionHandler(
        int bufferSize,
        BufferOverflowBehavior overflowBehavior,
        Func<T?, Task>? asyncFunction,
        Action<T?>? syncFunction) : IDisposable
    {
        private readonly ConcurrentQueue<T?> _queue = new();
        private Task? _worker;
        private readonly SemaphoreSlim _signal = new(0);
        private readonly CancellationTokenSource _cts = new();

        public void BufferInitial(IEnumerable<T?> initialValues)
        {
            foreach (var v in initialValues) _queue.Enqueue(v);
        }

        public void Start()
        {
            _worker = Task.Run(ProcessQueueAsync);
        }

        public void Enqueue(T? value)
        {
            lock (_queue)
            {
                if (bufferSize > 0 && _queue.Count >= bufferSize)
                {
                    if (overflowBehavior == BufferOverflowBehavior.DropOldest)
                    {
                        _queue.TryDequeue(out _);
                    }
                    else if (overflowBehavior == BufferOverflowBehavior.DropNewest)
                    {
                        return; // Do not enqueue if buffer is full and behavior is to drop newest
                    }
                }
                _queue.Enqueue(value);
            }
            _signal.Release();
        }

        private async Task ProcessQueueAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                await _signal.WaitAsync(_cts.Token);

                if (_queue.TryDequeue(out T? value))
                {
                    try
                    {
                        if (asyncFunction != null)
                        {
                            await asyncFunction(value);
                        }
                        else
                        {
                            syncFunction?.Invoke(value);
                        }
                    }
                    catch
                    {
                        //swallow
                    }
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _worker?.Wait();
            _cts.Dispose();
            _signal.Dispose();
        }
    }
}
