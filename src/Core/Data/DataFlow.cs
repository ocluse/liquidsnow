namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// A class that provides methods for creating and managing data flows.
/// </summary>
public static class DataFlow
{
    /// <summary>
    /// Creates a new data flow that emits values normally (as soon as they arrive).
    /// </summary>
    /// <param name="replayCount">The number of past values that will be replayed to each new subscriber on join.</param>
    /// <returns>A new <see cref="IMutableDataFlow{T}"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IMutableDataFlow<T> Create<T>(int replayCount = 0)
    {
        if (replayCount < 0) throw new ArgumentOutOfRangeException(nameof(replayCount), "Replay count must be non-negative.");
        return new DataFlow<T>(replayCount);
    }
}

internal sealed class DataFlow<T>(int replayCount) : IMutableDataFlow<T>
{
    private readonly HashSet<SubscriptionHandler<T>> _handlers = [];
    private readonly List<T> _history = [];
    private readonly object _lock = new();

    private bool _isPaused;
    private ResumeData? _resumeData;

    public bool Paused => _isPaused;

    private record ResumeData(T Value);

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

    public IDisposable Subscribe(Func<T, Task> subscriber, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        SubscriptionHandler<T> handler = new(bufferSize, overflowBehavior, subscriber, null);
        AddHandler(handler);
        return handler;
    }

    public IDisposable Subscribe(Action<T> subscriber, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        SubscriptionHandler<T> handler = new(bufferSize, overflowBehavior, null, subscriber);
        AddHandler(handler);
        return handler;
    }

    private void AddHandler(SubscriptionHandler<T> handler)
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

    public void Emit(T value)
    {
        lock (_lock)
        {
            if (_isPaused)
            {
                _resumeData = new ResumeData(value);
                return;
            }

            AppendHistory(value);
            Dispatch(value);
        }
    }

    private void AppendHistory(T value)
    {
        if (replayCount <= 0) return;
        _history.Add(value);
        if (_history.Count > replayCount)
            _history.RemoveAt(0);
    }

    private void Dispatch(T value)
    {
        foreach (var handler in _handlers)
        {
            handler.Enqueue(value);
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            foreach (var handler in _handlers)
            {
                handler.Dispose();
            }
            _handlers.Clear();
        }
    }
}
