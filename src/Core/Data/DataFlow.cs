using System.Collections.Concurrent;

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
    private readonly HashSet<SubscriptionHandler> _handlers = [];
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

            if(_resumeData == null)
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
        SubscriptionHandler handler = new(bufferSize, overflowBehavior, subscriber, null);
        AddHandler(handler);
        return handler;
    }

    public IDisposable Subscribe(Action<T> subscriber, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
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

    private sealed class SubscriptionHandler(
        int bufferSize,
        BufferOverflowBehavior overflowBehavior,
        Func<T, Task>? asyncFunction,
        Action<T>? syncFunction) : IDisposable
    {
        private readonly ConcurrentQueue<T> _queue = new();
        private Task? _worker;
        private readonly SemaphoreSlim _signal = new(0);
        private readonly CancellationTokenSource _cts = new();

        public void BufferInitial(IEnumerable<T> initialValues)
        {
            foreach (var v in initialValues) _queue.Enqueue(v);
        }

        public void Start()
        {
            _worker = Task.Run(ProcessQueueAsync);
        }

        public void Enqueue(T value)
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
                        return;
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
                        // swallow
                    }
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _signal.Release();
            _cts.Dispose();
            _signal.Dispose();
        }
    }
}
