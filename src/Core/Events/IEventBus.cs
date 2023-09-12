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
        Task Publish(IEvent ev, CancellationToken cancellationToken = default);
    }
}
