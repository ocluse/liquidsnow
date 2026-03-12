namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Manages a collection of <see cref="IDataFlow{T}"/> subscriptions that can all be cancelled at once.
/// </summary>
/// <remarks>
/// Use <c>Subscribe</c> to subscribe to flows and have the resulting subscriptions tracked by this scope.
/// Disposing the scope will dispose all tracked subscriptions.
/// Individual subscriptions can be cancelled early via <see cref="Cancel"/>.
/// </remarks>
public sealed class DataFlowScope : IDisposable
{
    private readonly List<IDisposable> _subscriptions = [];
    private readonly object _lock = new();
    private bool _disposed;

    /// <summary>
    /// Subscribes to the specified data flow with an async callback and tracks the subscription in this scope.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the flow.</typeparam>
    /// <param name="flow">The data flow to subscribe to.</param>
    /// <param name="subscriberFunc">The async callback to invoke when a value is emitted.</param>
    /// <param name="bufferSize">The total number of items that will be buffered while the subscriber is still executing and a new value is received.</param>
    /// <param name="overflowBehavior">Determines how the buffer will behave when the buffer limit is reached.</param>
    /// <returns>
    /// An <see cref="IDisposable"/> representing the individual subscription.
    /// Pass it to <see cref="Cancel"/> to cancel this subscription before the scope is disposed.
    /// </returns>
    /// <exception cref="ObjectDisposedException">When the scope has already been disposed.</exception>
    public IDisposable Subscribe<T>(
        IDataFlow<T> flow,
        Func<T?, Task> subscriberFunc,
        int bufferSize = 0,
        BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var subscription = flow.Subscribe(subscriberFunc, bufferSize, overflowBehavior);
        lock (_lock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _subscriptions.Add(subscription);
        }
        return subscription;
    }

    /// <summary>
    /// Subscribes to the specified data flow with a sync callback and tracks the subscription in this scope.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the flow.</typeparam>
    /// <param name="flow">The data flow to subscribe to.</param>
    /// <param name="subscriberFunc">The callback to invoke when a value is emitted.</param>
    /// <param name="bufferSize">The total number of items that will be buffered while the subscriber is still executing and a new value is received.</param>
    /// <param name="overflowBehavior">Determines how the buffer will behave when the buffer limit is reached.</param>
    /// <returns>
    /// An <see cref="IDisposable"/> representing the individual subscription.
    /// Pass it to <see cref="Cancel"/> to cancel this subscription before the scope is disposed.
    /// </returns>
    /// <exception cref="ObjectDisposedException">When the scope has already been disposed.</exception>
    public IDisposable Subscribe<T>(
        IDataFlow<T> flow,
        Action<T?> subscriberFunc,
        int bufferSize = 0,
        BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var subscription = flow.Subscribe(subscriberFunc, bufferSize, overflowBehavior);
        lock (_lock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _subscriptions.Add(subscription);
        }
        return subscription;
    }

    /// <summary>
    /// Cancels a single tracked subscription, disposing it and removing it from this scope.
    /// </summary>
    /// <param name="subscription">The subscription handle previously returned by <c>Subscribe</c>.</param>
    /// <exception cref="ObjectDisposedException">When the scope has already been disposed.</exception>
    public void Cancel(IDisposable subscription)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_lock)
        {
            _subscriptions.Remove(subscription);
        }
        subscription.Dispose();
    }

    /// <summary>
    /// Disposes all tracked subscriptions and marks this scope as disposed.
    /// </summary>
    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed) return;
            _disposed = true;
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
            _subscriptions.Clear();
        }
    }
}
