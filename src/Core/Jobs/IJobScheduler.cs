namespace Ocluse.LiquidSnow.Jobs
{
    /// <summary>
    /// Provides utility methods for scheduling and dispatching jobs.
    /// </summary>
    public interface IJobScheduler
    {
        /// <summary>
        /// Schedules a job to be run at a particular time.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="IDisposable"/> that can be used to cancel the job.
        /// </returns>
        IDisposable Schedule(IJob job);

        /// <summary>
        /// Cancels a job with the provided id. If the job has not yet been run, it will be removed from the queue.
        /// </summary>
        /// <returns>
        /// True if the job was cancelled, false if the job was not found.
        /// </returns>
        bool Cancel(object id);
    }
}
