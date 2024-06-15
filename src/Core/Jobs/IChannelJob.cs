namespace Ocluse.LiquidSnow.Jobs
{
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
