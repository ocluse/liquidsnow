namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Defines a data flow that allows subscribers to receive updates of a specific type.
/// </summary>
/// <typeparam name="T">The type of value emitted by this flow</typeparam>
/// <remarks>
/// Disposing the data flow will unsubscribe all subscribers and stop the flow.
/// </remarks>
public interface IDataFlow<T> : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the data flow is currently paused.
    /// </summary>
    bool Paused { get; }

    /// <summary>
    /// Gets a value indicating the mode of the data flow, which determines how values are emitted and handled.
    /// </summary>
    DataFlowMode Mode { get; }

    /// <summary>
    /// Adds a subscriber/consumer to the data flow and returns an <see cref="IDisposable"/> that can be used to unsubscribe.
    /// </summary>
    /// <param name="subscriberFunc">The subscriber method to receive updates with new values</param>
    /// <param name="bufferSize">The total number of items that will be buffered while the subscriber is still executing and a new value is received.</param>
    /// <param name="overflowBehavior">Determines how the buffer will behave when the buffer limit is reached.</param>
    /// <returns>
    /// An <see cref="IDisposable"/> that can be used to unsubscribe from the data flow.
    /// </returns>
    IDisposable Subscribe(Func<T?, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest);

    ///<inheritdoc cref="Subscribe(Func{T?, Task}, int, BufferOverflowBehavior)"/>
    IDisposable Subscribe(Action<T?> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest);

    /// <summary>
    /// Emits a value to the data flow, which will be sent to all subscribers.
    /// </summary>
    /// <param name="value">The value to be emitted</param>
    void Emit(T? value);

    /// <summary>
    /// Pauses the data flow, preventing any new values from being emitted until it is resumed.
    /// </summary>
    /// <remarks>
    /// The last emitted value while paused will be stored and emitted when the flow is resumed.
    /// </remarks>
    /// <exception cref="InvalidOperationException">When the data flow is already paused.</exception>"
    void Pause();

    /// <summary>
    /// Resumes the data flow after it has been paused.
    /// </summary>
    /// <remarks>
    /// The last emitted value while paused will be sent to all subscribers when the flow is resumed.
    /// </remarks>
    /// <exception cref="InvalidOperationException">When the data is resumed but was not in a paused state.</exception>
    void Resume();
}