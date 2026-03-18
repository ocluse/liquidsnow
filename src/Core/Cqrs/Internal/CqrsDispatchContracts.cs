namespace Ocluse.LiquidSnow.Cqrs.Internal;

/// <summary>
/// Represents CQRS dispatch categories.
/// </summary>
public enum DispatchKind
{
    /// <summary>
    /// Command dispatch.
    /// </summary>
    Command,

    /// <summary>
    /// Query dispatch.
    /// </summary>
    Query
}

/// <summary>
/// Identifies a dispatch route by kind, request type and result type.
/// </summary>
public readonly record struct DispatchKey(DispatchKind Kind, Type RequestType, Type ResultType);

/// <summary>
/// Describes an executable dispatch route.
/// </summary>
public sealed record DispatchDescriptor(
    DispatchKind Kind,
    Type RequestType,
    Type ResultType,
    Func<object, IServiceProvider, CancellationToken, Task<object>> ExecuteAsync);

/// <summary>
/// Provides generated dispatch descriptors to the runtime dispatcher.
/// </summary>
public interface ICqrsDispatchContributor
{
    /// <summary>
    /// Gets all dispatch descriptors contributed by an assembly.
    /// </summary>
    IEnumerable<DispatchDescriptor> Descriptors { get; }
}
