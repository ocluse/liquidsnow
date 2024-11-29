using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocluse.LiquidSnow.Events;
using Ocluse.LiquidSnow.Events.Internal;
using System.Reflection;

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

    /// <summary>
    /// Creates a new instance of the <see cref="EventBusBuilder"/>, adds essential EventBus services and adds handlers from the provided assembly.
    /// </summary>
    public EventBusBuilder(IServiceCollection services, Assembly assembly, ServiceLifetime listenerLifetime = ServiceLifetime.Transient)
    {
        Services = services;
        AddCore();
        AddListeners(assembly, listenerLifetime);
    }

    private void AddCore()
    {
        Services.TryAddSingleton<EventDescriptorCache>();
        Services.TryAddTransient<IEventBus, EventBus>();
    }

    /// <summary>
    /// Adds listeners from the provided assembly.
    /// </summary>
    public EventBusBuilder AddListeners(Assembly assembly, ServiceLifetime listenerLifetime = ServiceLifetime.Transient)
    {
        Services.TryAddImplementersOfGenericAsImplemented(typeof(IEventListener<>), assembly, listenerLifetime);
        return this;
    }

    /// <summary>
    /// Adds event listeners from the provided assemblies.
    /// </summary>
    public EventBusBuilder AddListeners(IEnumerable<Assembly> assemblies, ServiceLifetime listenerLifetime = ServiceLifetime.Transient)
    {        
        foreach (var assembly in assemblies)
        {
            AddListeners(assembly, listenerLifetime);
        }
        
        return this;
    }

    /// <summary>
    /// Adds the specified event listeners to the service collection.
    /// </summary>
    public EventBusBuilder AddListener<TEvent, TListener>(ServiceLifetime listenerLifetime = ServiceLifetime.Transient) 
        where TListener : IEventListener<TEvent>
    {
        ServiceDescriptor descriptor = new(typeof(IEventListener<TEvent>), typeof(TListener), listenerLifetime);
        Services.TryAdd(descriptor);

        return this;
    }
}