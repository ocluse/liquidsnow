using System.Text.Json;

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

    /// <summary>Gets the serializer settings used for durable job payloads.</summary>
    public JsonSerializerOptions SerializerOptions { get; } = new(JsonSerializerDefaults.Web);

    /// <summary>Gets or sets how frequently an idle durable store is checked.</summary>
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>Gets or sets the execution lease duration. Active leases are renewed.</summary>
    public TimeSpan LeaseDuration { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>Gets or sets the maximum number of jobs claimed in one store operation.</summary>
    public int ClaimBatchSize { get; set; } = 32;

    /// <summary>Gets or sets the maximum number of handlers executed concurrently.</summary>
    public int MaximumConcurrency { get; set; } = Math.Max(1, Environment.ProcessorCount);
}
