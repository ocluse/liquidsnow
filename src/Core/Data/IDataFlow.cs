namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Defines a read-only view of a data flow that allows subscribing to receive emitted values.
/// </summary>
/// <typeparam name="T">The type of value emitted by this flow.</typeparam>
/// <remarks>
/// To emit values or control the flow lifecycle (including disposal), see <see cref="IMutableDataFlow{T}"/>.
/// </remarks>
public interface IDataFlow<T>
{
    /// <summary>
    /// Gets a value indicating whether the data flow is currently paused.
    /// </summary>
    bool Paused { get; }

    /// <summary>
    /// Adds a subscriber to the data flow and returns an <see cref="IDisposable"/> that can be used to unsubscribe.
    /// </summary>
    /// <param name="subscriberFunc">The async callback to invoke when a value is emitted.</param>
    /// <param name="bufferSize">The total number of items that will be buffered while the subscriber is still executing and a new value is received.</param>
    /// <param name="overflowBehavior">Determines how the buffer will behave when the buffer limit is reached.</param>
    /// <returns>An <see cref="IDisposable"/> that can be used to unsubscribe from the data flow.</returns>
    IDisposable Subscribe(Func<T, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest);

    /// <inheritdoc cref="Subscribe(Func{T, Task}, int, BufferOverflowBehavior)"/>
    IDisposable Subscribe(Action<T> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest);
}
