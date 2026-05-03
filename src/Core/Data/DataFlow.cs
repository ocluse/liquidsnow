namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Represents a mutable data flow that allows values to be emitted and observed by multiple subscribers, with optional
/// replay of recent values to new subscribers.
/// </summary>
/// <remarks>
/// <see cref="DataFlow{T}"/> supports both synchronous and asynchronous subscribers. When a new subscriber joins, it
/// receives up to the specified number of most recent values, if any have been emitted. The data flow can be paused and
/// resumed; while paused, new values are not dispatched to subscribers but the most recent value is stored and sent
/// upon resuming. Thread safety is ensured for all public operations. Disposing the data flow unsubscribes all
/// subscribers and prevents further emissions.
/// </remarks>
/// <typeparam name="T">The type of values transmitted through the data flow.</typeparam>
public sealed class DataFlow<T> : IDataFlow<T>, IDisposable
{
    private readonly HashSet<SubscriptionHandler<T>> _handlers = [];
    private readonly List<T> _history = [];
    private readonly object _lock = new();
    private readonly int _replayCount;

    private bool _isPaused;
    private bool _disposed;
    private ResumeData? _resumeData;

    ///<inheritdoc/>
    public bool Paused => _isPaused;

    private record ResumeData(T Value);

    /// <summary>
    /// Creates a new data flow that emits values normally (as soon as they arrive).
    /// </summary>
    /// <param name="replayCount">The number of past values that will be replayed to each new subscriber on join.</param>
    /// <returns>A new <see cref="DataFlow{T}"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public DataFlow(int replayCount = 0)
    {
        if (replayCount < 0) throw new ArgumentOutOfRangeException(nameof(replayCount), "Replay count must be non-negative.");
        _replayCount = replayCount;
    }

    /// <summary>
    /// Pauses the data flow, preventing any new values from being dispatched to subscribers until it is resumed.
    /// </summary>
    /// <remarks>
    /// The last value emitted while paused will be stored and dispatched when the flow is resumed.
    /// </remarks>
    /// <exception cref="InvalidOperationException">When the data flow is already paused.</exception>
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

    /// <summary>
    /// Resumes the data flow after it has been paused.
    /// </summary>
    /// <remarks>
    /// The last value emitted while paused will be dispatched to all subscribers when the flow is resumed.
    /// </remarks>
    /// <exception cref="InvalidOperationException">When the data flow is not currently paused.</exception>
    public void Resume()
    {
        if (!_isPaused)
        {
            throw new InvalidOperationException("Cannot resume a flow that is not paused.");
        }

        T next;

        lock (_lock)
        {
            _isPaused = false;

            if (_resumeData == null)
            {
                return;
            }

            next = _resumeData.Value;
            _resumeData = null;
        }

        Emit(next);
    }

    ///<inheritdoc/>
    public IDisposable Subscribe(Func<T, Task> subscriber, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        SubscriptionHandler<T> handler = new(bufferSize, overflowBehavior, subscriber, null);
        AddHandler(handler);
        return handler;
    }

    ///<inheritdoc/>
    public IDisposable Subscribe(Action<T> subscriber, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        SubscriptionHandler<T> handler = new(bufferSize, overflowBehavior, null, subscriber);
        AddHandler(handler);
        return handler;
    }

    /// <summary>
    /// Emits a value to the data flow, which will be sent to all subscribers.
    /// </summary>
    /// <param name="value">The value to emit.</param>
    public void Emit(T value)
    {
        lock (_lock)
        {
            if (_disposed) return;

            if (_isPaused)
            {
                _resumeData = new ResumeData(value);
                return;
            }

            AppendHistory(value);
            Dispatch(value);
        }
    }

    private void AddHandler(SubscriptionHandler<T> handler)
    {
        lock (_lock)
        {
            if (_replayCount > 0)
            {
                var start = Math.Max(0, _history.Count - _replayCount);
                var recent = _history.Skip(start).ToList();
                handler.BufferInitial(recent);
            }
            _handlers.Add(handler);
        }
        handler.Start();
    }

    private void AppendHistory(T value)
    {
        if (_replayCount <= 0) return;
        _history.Add(value);
        if (_history.Count > _replayCount)
            _history.RemoveAt(0);
    }

    private void Dispatch(T value)
    {
        foreach (var handler in _handlers)
        {
            handler.Enqueue(value);
        }
    }

    ///<inheritdoc/>
    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed) return;
            _disposed = true;
            foreach (var handler in _handlers)
            {
                handler.Dispose();
            }
            _handlers.Clear();
        }
    }
}
