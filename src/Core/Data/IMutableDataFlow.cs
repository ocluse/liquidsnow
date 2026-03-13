namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Defines a mutable data flow that can emit values and have its lifecycle controlled.
/// </summary>
/// <typeparam name="T">The type of value emitted by this flow.</typeparam>
/// <remarks>
/// To subscribe to a flow without the ability to emit or control its lifecycle, use <see cref="IDataFlow{T}"/>.
/// </remarks>
public interface IMutableDataFlow<T> : IDataFlow<T>
{
    /// <summary>
    /// Emits a value to the data flow, which will be sent to all subscribers.
    /// </summary>
    /// <param name="value">The value to emit.</param>
    void Emit(T value);

    /// <summary>
    /// Pauses the data flow, preventing any new values from being dispatched to subscribers until it is resumed.
    /// </summary>
    /// <remarks>
    /// The last value emitted while paused will be stored and dispatched when the flow is resumed.
    /// </remarks>
    /// <exception cref="InvalidOperationException">When the data flow is already paused.</exception>
    void Pause();

    /// <summary>
    /// Resumes the data flow after it has been paused.
    /// </summary>
    /// <remarks>
    /// The last value emitted while paused will be dispatched to all subscribers when the flow is resumed.
    /// </remarks>
    /// <exception cref="InvalidOperationException">When the data flow is not currently paused.</exception>
    void Resume();
}
