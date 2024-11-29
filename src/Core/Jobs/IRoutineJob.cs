namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// Represents a <see cref="IJob"/> that once started, is run at a regular interval.
/// </summary>
public interface IRoutineJob : IJob
{
    /// <summary>
    /// The interval at which the job should be run.
    /// </summary>
    public TimeSpan Interval { get; }
}
