namespace Ocluse.LiquidSnow.Events
{
    ///<inheritdoc/>
    public interface ITimestampedEvent : IEvent
    {
        /// <summary>
        /// The time when the event occurred.
        /// </summary>
        DateTime Timestamp { get; }
    }
}
