namespace Ocluse.LiquidSnow.Events;

/// <summary>
/// Defines methods for publishing events to registered listeners.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an event to all registered listeners capable of handling it and waits until they all finish execution.
    /// </summary>
    Task PublishAsync<TEvent>(TEvent e, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes an event to all registered listeners capable of handling it, returning immediately after the event is dispatched.
    /// </summary>
    /// <remarks>
    /// This method creates a new service scope to safely execute the event handlers, in case the current scope is prematurely disposed.
    /// </remarks>
    void Publish<TEvent>(TEvent e);
}
