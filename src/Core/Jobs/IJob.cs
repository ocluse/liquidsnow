using System;

namespace Ocluse.LiquidSnow.Jobs
{
    /// <summary>
    /// Represents a job that can be run by a <see cref="IJobHandler{T}"/> at a particular time.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// A unique identifier for the job.
        /// </summary>
        object Id { get; }

        /// <summary>
        /// The time at which the job should be run.
        /// </summary>
        DateTime Start { get; }
    }
}
