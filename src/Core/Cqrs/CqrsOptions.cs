namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Configuration options for the CQRS subsystem.
/// </summary>
public class CqrsOptions
{
    /// <summary>
    /// When <see langword="true"/>, the dispatcher will walk up the inheritance
    /// chain of a command/query type to find a registered handler if no handler
    /// is registered for the exact type. Default is <see langword="false"/>.
    /// </summary>
    public bool EnablePolymorphicResolution { get; set; }
}
