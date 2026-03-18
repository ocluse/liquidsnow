using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocluse.LiquidSnow.Events;
using Ocluse.LiquidSnow.Events.Internal;

namespace Ocluse.LiquidSnow.DependencyInjection;

/// <summary>
/// Provides methods for configuring EventBus in a service collection.
/// </summary>
public class EventBusBuilder
{
    /// <summary>
    /// Gets the service collection where the handlers are configured.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Creates a new instance of <see cref="EventBusBuilder"/> and adds essential EventBus services.
    /// </summary>
    public EventBusBuilder(IServiceCollection services)
    {
        Services = services;
        AddCore();
    }

    private void AddCore()
    {
        Services.TryAddEnumerable(ServiceDescriptor.Singleton<IEventDispatchContributor, EmptyEventDispatchContributor>());
        Services.TryAddTransient<EventBus>();
        Services.TryAddTransient<IEventBus, EventBus>();
    }
}
