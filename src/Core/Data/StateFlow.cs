namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Defines a mutable state flow that can be observed by multiple subscbers for state changes.
/// </summary>
/// <remarks>
/// The current value is always replayed to new subscribers.
/// When a new value is set, it is compared to the current value using the provided equality comparer (or default if none is provided). 
/// If they are equal, the update is ignored and subscribers are not notified. 
/// Otherwise, the new value replaces the current one and is dispatched to all subscribers.
/// </remarks>
/// <typeparam name="T">The type of value held by this flow.</typeparam>
/// <remarks>
/// Creates a new state flow with the given initial value.
/// </remarks>
/// <param name="initialValue">The initial value replayed to subscribers until the first update.</param>
/// <param name="comparer">
/// An optional equality comparer used to determine whether a new value differs from the current one.
/// Defaults to <see cref="EqualityComparer{T}.Default"/>.
/// </param>
/// <returns>A new <see cref="StateFlow{T}"/>.</returns>
public sealed class StateFlow<T>(T initialValue, IEqualityComparer<T>? comparer) : IStateFlow<T>, IDisposable
{
    private readonly HashSet<SubscriptionHandler<T>> _handlers = [];
    private readonly object _lock = new();
    private readonly IEqualityComparer<T> _comparer = comparer ?? EqualityComparer<T>.Default;
    private T _value = initialValue;
    private bool _disposed;

    /// <inheritdoc/>
    public bool Paused => false;

    /// <summary>
    /// Gets or sets the current value of the state flow.
    /// </summary>
    /// <remarks>
    /// Setting this property updates the value only if it differs from the current value, as
    /// determined by the equality comparer. If the flow disposed, setting the value has no
    /// effect.
    /// </remarks>
    public T Value
    {
        get
        {
            lock (_lock)
            {
                return _value;
            }
        }
        set
        {
            lock (_lock)
            {
                if (_disposed) return;
                if (_comparer.Equals(_value, value)) return;
                _value = value;
                Dispatch(value);
            }
        }
    }

    /// <inheritdoc/>
    public IDisposable Subscribe(Func<T, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        SubscriptionHandler<T> handler = new(bufferSize, overflowBehavior, subscriberFunc, null);
        AddHandler(handler);
        return handler;
    }

    /// <inheritdoc/>
    public IDisposable Subscribe(Action<T> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        SubscriptionHandler<T> handler = new(bufferSize, overflowBehavior, null, subscriberFunc);
        AddHandler(handler);
        return handler;
    }

    private void AddHandler(SubscriptionHandler<T> handler)
    {
        lock (_lock)
        {
            handler.BufferInitial([_value]);
            _handlers.Add(handler);
        }
        handler.Start();
    }

    private void Dispatch(T value)
    {
        foreach (var handler in _handlers)
        {
            handler.Enqueue(value);
        }
    }

    /// <inheritdoc/>
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
