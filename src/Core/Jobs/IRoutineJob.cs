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
    /// Represents a job that is run on a specific channel.
    /// </summary>
    public interface IChannelJob : IJob
    {
        /// <summary>
        /// The channel ID that the job is run on.
        /// </summary>
        public object ChannelId { get; }
    }
}
