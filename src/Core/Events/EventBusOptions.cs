namespace Ocluse.LiquidSnow.Events;

/// <summary>
/// Configuration options for the EventBus subsystem.
/// </summary>
public class EventBusOptions
{
    /// <summary>
    /// When <see langword="true"/>, the event bus will walk up the inheritance
    /// chain of an event type to find registered listeners if no listeners
    /// are registered for the exact type. Default is <see langword="false"/>.
    /// </summary>
    public bool EnablePolymorphicResolution { get; set; }
}
