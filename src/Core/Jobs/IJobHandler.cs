namespace Ocluse.LiquidSnow.Jobs
{
    /// <summary>
    /// Provides a contract for creating handlers for jobs, which will execute the job.
    /// </summary>
    public interface IJobHandler<in T> where T : IJob
    {
        /// <summary>
        /// Executes the provided job.
        /// </summary>
        Task Handle(T job, long tick, CancellationToken cancellationToken);
    }
}
