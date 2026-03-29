using System.Collections.Concurrent;

namespace Ocluse.LiquidSnow.Data;

internal sealed class SubscriptionHandler<T>(
    int bufferSize,
    BufferOverflowBehavior overflowBehavior,
    Func<T, Task>? asyncFunction,
    Action<T>? syncFunction) : IDisposable
{
    private readonly ConcurrentQueue<T> _queue = new();
    private readonly SemaphoreSlim _signal = new(0);
    private readonly CancellationTokenSource _cts = new();
    private bool _disposed;
    
    public void BufferInitial(IEnumerable<T> initialValues)
    {
        foreach (var v in initialValues)
        {
            Enqueue(v);
        }
    }

    public void Start()
    {
        _ = Task.Run(ProcessQueueAsync);
    }

    public void Enqueue(T value)
    {
        lock (_queue)
        {
            if (_disposed) return;

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
        try
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
                        // swallow subscriber exceptions
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // clean exit on cancellation
        }
    }

    public void Dispose()
    {
        lock (_queue)
        {
            if (_disposed) return;
            _disposed = true;
        }

        _cts.Cancel();
        _signal.Release();
        _cts.Dispose();
        _signal.Dispose();
    }
}
