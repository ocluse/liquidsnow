namespace Ocluse.LiquidSnow.Events.Internal
{
    /// <summary>
    /// Options used to configure how the Event Bus and event handlers should be added to a DI Container
    /// </summary>
    internal class EventBusOptions
    {
        public PublishStrategy PublishStrategy { get; set; }
    }
}