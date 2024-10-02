namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// A variant of the <see cref="IRoutineJob"/>
/// </summary>
/// <remarks>
/// Unlike in the standard <see cref="IRoutineJob"/>, where the interval determines when the job is always run,
/// the interval in this type of job determines the delay between the end of the previous job and the start of the next job.
/// </remarks>
public interface ITaskSeriesJob : IRoutineJob
{
}
