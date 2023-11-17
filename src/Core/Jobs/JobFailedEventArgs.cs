namespace Ocluse.LiquidSnow.Jobs
{
    /// <summary>
    /// Represents an error that occurred while executing a job.
    /// </summary>
    /// <remarks>
    /// Creates a new instance of <see cref="JobFailedEventArgs"/>.
    /// </remarks>
    /// <param name="job"></param>
    /// <param name="exception"></param>
    public class JobFailedEventArgs(IJob job, Exception exception) : EventArgs
    {
        /// <summary>
        /// The job that failed.
        /// </summary>
        public IJob Job { get; } = job;

        /// <summary>
        /// The exception that was thrown while executing the job.
        /// </summary>
        public Exception Exception { get; } = exception;
    }
}
