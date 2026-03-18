namespace Ocluse.LiquidSnow.Events.Internal;

/// <summary>
/// Identifies an event dispatch route by event type.
/// </summary>
public readonly record struct EventDispatchKey(Type EventType);

/// <summary>
/// Describes an executable event dispatch route.
/// </summary>
public sealed record EventDispatchDescriptor(
    Type EventType,
    Func<object, IServiceProvider, CancellationToken, Task> ExecuteAsync);

/// <summary>
/// Provides generated event dispatch descriptors to the runtime event bus.
/// </summary>
public interface IEventDispatchContributor
{
    /// <summary>
    /// Gets all event dispatch descriptors contributed by an assembly.
    /// </summary>
    IEnumerable<EventDispatchDescriptor> Descriptors { get; }
}
