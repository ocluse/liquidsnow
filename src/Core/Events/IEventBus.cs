namespace Ocluse.LiquidSnow.Events;

/// <summary>
/// Defines methods for publishing events to registered listeners.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an event to all registered listeners capable of handling it and waits until they all finish execution.
    /// </summary>
    /// <remarks>
    /// Exceptions thrown by the event listeners are wrapped in an <see cref="AggregateException"/> and rethrown after all listeners have finished executing.
    /// </remarks>
    Task PublishAsync<TEvent>(TEvent e, CancellationToken cancellationToken = default);

    ///<inheritdoc cref="PublishAsync{TEvent}(TEvent, CancellationToken)"/>
    Task PublishAsync(object e, Type eventType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes an event to all registered listeners capable of handling it, returning immediately after the event is dispatched without waiting for completion.
    /// </summary>
    /// <remarks>
    /// This method creates a new service scope to safely execute the event listeners, in case the current scope is prematurely disposed.
    /// </remarks>
    void Publish<TEvent>(TEvent e);

    ///<inheritdoc cref="Publish{TEvent}(TEvent)"/>
    void Publish(object e, Type eventType);
}
