namespace Ocluse.LiquidSnow.Events;

/// <summary>
/// Defines methods for handling a specific type of events.
/// </summary>
public interface IEventListener<T>
{
    /// <summary>
    /// Handles the specified event.
    /// </summary>
    Task HandleAsync(T e, CancellationToken cancellationToken = default);
}
