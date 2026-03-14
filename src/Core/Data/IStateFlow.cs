namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Defines a read-only view of a state flow — a data flow that always holds a current value
/// and replays it immediately to each new subscriber.
/// </summary>
/// <typeparam name="T">The type of value held by this flow.</typeparam>
/// <remarks>
/// A state flow is never paused; <see cref="IDataFlow{T}.Paused"/> always returns <c>false</c>.
/// To update the value or dispose the flow, see <see cref="IMutableStateFlow{T}"/>.
/// </remarks>
public interface IStateFlow<T> : IDataFlow<T>
{
    /// <summary>
    /// Gets the current value held by the state flow.
    /// </summary>
    T Value { get; }
}
