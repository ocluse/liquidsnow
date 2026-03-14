namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Provides a method for creating state flows.
/// </summary>
public static class StateFlow
{
    /// <summary>
    /// Creates a new state flow with the given initial value.
    /// </summary>
    /// <typeparam name="T">The type of value held by the flow.</typeparam>
    /// <param name="initialValue">The initial value replayed to subscribers until the first update.</param>
    /// <param name="comparer">
    /// An optional equality comparer used to determine whether a new value differs from the current one.
    /// Defaults to <see cref="EqualityComparer{T}.Default"/>.
    /// </param>
    /// <returns>A new <see cref="IMutableStateFlow{T}"/>.</returns>
    public static IMutableStateFlow<T> Create<T>(T initialValue, IEqualityComparer<T>? comparer = null)
    {
        return new StateFlow<T>(initialValue, comparer ?? EqualityComparer<T>.Default);
    }
}

internal sealed class StateFlow<T>(T initialValue, IEqualityComparer<T> comparer) : IMutableStateFlow<T>
{
    private readonly HashSet<SubscriptionHandler<T>> _handlers = [];
    private readonly object _lock = new();

    private T _value = initialValue;

    /// <inheritdoc/>
    public bool Paused => false;

    /// <inheritdoc/>
    public T Value
    {
        get
        {
            lock (_lock)
            {
                return _value;
            }
        }
    }

    /// <inheritdoc/>
    public void Update(T value)
    {
        lock (_lock)
        {
            if (comparer.Equals(_value, value)) return;
            _value = value;
            Dispatch(value);
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
            foreach (var handler in _handlers)
            {
                handler.Dispose();
            }
            _handlers.Clear();
        }
    }
}
