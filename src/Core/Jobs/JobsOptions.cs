namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// Configuration options for the Jobs subsystem.
/// </summary>
public class JobsOptions
{
    /// <summary>
    /// When <see langword="true"/>, the dispatcher will walk up the inheritance
    /// chain of a job type to find a registered handler if no handler
    /// is registered for the exact type. Default is <see langword="false"/>.
    /// </summary>
    public bool EnablePolymorphicResolution { get; set; }
}
