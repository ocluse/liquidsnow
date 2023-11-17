using Ocluse.LiquidSnow.Events;

namespace Ocluse.LiquidSnow.Jobs
{
    /// <summary>
    /// A job that can be handled by multiple handlers
    /// </summary>
    public interface IMulticastJob : IJob
    {
        /// <summary>
        /// When true, all handlers for the job will be invoked in parallel.
        /// </summary>
        bool ExecuteParallel { get; }
    }

    /// <summary>
    /// A strategy for executing <see cref="IMulticastJob"/> jobs
    /// </summary>
    public enum MulticastStrategy
    {
        /// <summary>
        /// Handlers will be executed sequentially
        ///</summary>
        Sequential,
        /// <summary>
        /// Handlers will be executed in parallel
        /// </summary>
        Parallel
    }
}

