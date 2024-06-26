﻿namespace Ocluse.LiquidSnow.Jobs
{
    /// <summary>
    /// Provides utility methods for scheduling and dispatching jobs.
    /// </summary>
    public interface IJobScheduler
    {
        /// <summary>
        /// Schedules a job to be run at a particular time.
        /// </summary>
        /// <remarks>
        /// Scheduled jobs may be run in parallel if they have the same start time.
        /// </remarks>
        /// <returns>
        /// Returns an <see cref="IDisposable"/> that can be used to cancel the job.
        /// </returns>
        IDisposable Schedule(IJob job);

        /// <summary>
        /// Schedules a job to be added to the execution queue at a particular time.
        /// </summary>
        /// <param name="job"></param>
        /// <remarks>
        /// Queued jobs will always be run one at a time, in the order they were queued, even if they have the same start time.
        /// </remarks>
        /// <returns>
        /// Returns an <see cref="IDisposable"/> that can be used to cancel the job.
        /// </returns>
        IDisposable Queue(IQueueJob job);

        /// <summary>
        /// Cancels a job with the provided id or prevents it from running.
        /// </summary>
        /// <returns>
        /// True if the job was cancelled, false if the job was not found.
        /// </returns>
        bool Cancel(object id);

        /// <inheritdoc cref="Cancel(object)"/>
        /// <remarks>
        /// This is the way to cancel jobs that were queued via <see cref="Queue(IQueueJob)"/>
        /// </remarks>
        bool Cancel(object queueId, object id);
    }
}
