using System.Threading;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Events
{
    /// <summary>
    /// Provides utility methods for publishing events to various listeners/handlers
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Invokes any listeners of the specified type of event
        /// </summary>
        /// <param name="ev">The event to publish</param>
        /// <param name="strategy">The strategy to use when publishing the event. If none is specified, the default is used</param>
        /// <param name="cancellationToken">A token to request cancellation of the operation</param>
        Task Publish(IEvent ev, PublishStrategy? strategy = null, CancellationToken cancellationToken = default);
    }
}
