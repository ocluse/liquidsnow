namespace Ocluse.LiquidSnow.Jobs
{
    /// <summary>
    /// Represents a job that is executed on a specific queue.
    /// </summary>
    /// <remarks>
    /// When the queue is empty, the job will be run immediately.
    /// Otherwise, the job will be run after all other jobs ahead in the queue have been run.
    /// </remarks>
    public interface IQueueJob : IJob
    {
        /// <summary>
        /// The queue that the job is scheduled to run on.
        /// </summary>
        public object QueueId { get; }
    }
}
