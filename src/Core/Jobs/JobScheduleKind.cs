namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// Describes how a persisted job is scheduled.
/// </summary>
public enum JobScheduleKind
{
    /// <summary>
    /// The job is removed after its first execution.
    /// </summary>
    OneTime,

    /// <summary>
    /// The next occurrence is calculated from the previously scheduled time.
    /// </summary>
    FixedRate,

    /// <summary>
    /// The next occurrence is calculated from the completion time.
    /// </summary>
    TaskSeries
}
