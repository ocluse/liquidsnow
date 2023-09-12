using System;

namespace Ocluse.LiquidSnow.Events
{
    ///<inheritdoc/>
    public interface ITimedEvent : IEvent
    {
        /// <summary>
        /// The time when the event occurred.
        /// </summary>
        DateTime Timestamp { get; }
    }
}
