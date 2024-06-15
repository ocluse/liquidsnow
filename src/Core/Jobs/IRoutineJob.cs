namespace Ocluse.LiquidSnow.Jobs
{
    /// <summary>
    /// Represents a job that, once started, is run at a regular interval.
    /// </summary>
    public interface IRoutineJob : IJob
    {
        /// <summary>
        /// The interval at which the job should be run.
        /// </summary>
        public TimeSpan Interval { get; }
    }

    /// <summary>
    /// A variant of the <see cref="IRoutineJob"/>
    /// </summary>
    /// <remarks>
    /// Unlike in the standard <see cref="IRoutineJob"/>, where the interval determines when the job is always run,
    /// the interval in this type of job determines the delay between the end of the previous job and the start of the next job.
    /// </remarks>
    public interface ITaskSeriesJob : IJob
    {
        /// <summary>
        /// The interval at which the job should be run.
        /// </summary>
        public TimeSpan Interval { get; }
    }
}
