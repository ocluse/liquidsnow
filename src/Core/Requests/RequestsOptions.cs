namespace Ocluse.LiquidSnow.Requests;

/// <summary>
/// Configuration options for the Requests subsystem.
/// </summary>
public class RequestsOptions
{
    /// <summary>
    /// When <see langword="true"/>, the dispatcher will walk up the inheritance
    /// chain of a request type to find a registered handler if no handler
    /// is registered for the exact type. Default is <see langword="false"/>.
    /// </summary>
    public bool EnablePolymorphicResolution { get; set; }
}
