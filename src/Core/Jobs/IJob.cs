namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// Represents a job that is meant to run at a particular time.
/// </summary>
public interface IJob
{
    /// <summary>
    /// A unique identifier used to cancel or monitor the the job.
    /// </summary>
    object Id { get; }

    /// <summary>
    /// The time at which the job should be run.
    /// </summary>
    DateTimeOffset Start { get; }
}
