namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Defines a mutable state flow that holds a current value, replays it to new subscribers,
/// and skips consecutive equal values.
/// </summary>
/// <typeparam name="T">The type of value held by this flow.</typeparam>
/// <remarks>
/// To subscribe without the ability to update or dispose, use <see cref="IStateFlow{T}"/>.
/// A state flow does not support <c>Pause</c> or <c>Resume</c>.
/// </remarks>
public interface IMutableStateFlow<T> : IStateFlow<T>, IDisposable
{
    /// <summary>
    /// Updates the current value of the state flow.
    /// </summary>
    /// <param name="value">The new value. If it equals the current value the call is a no-op.</param>
    void Update(T value);
}
