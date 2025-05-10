using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.DependencyInjection;

/// <summary>
/// Extension methods to add CQRS, Event, and Orchestration handlers and dispatchers to a DI container.
/// </summary>
public static class BuilderExtensions
{
    #region CQRS

    /// <summary>
    /// Adds the CQRS dispatchers and handlers implemented in the calling assembly.
    /// </summary>
    public static CqrsBuilder AddCqrs(this IServiceCollection services, ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        return services.AddCqrs(Assembly.GetCallingAssembly(), handlerLifetime);
    }

    /// <summary>
    /// Adds the CQRS dispatchers and handlers from the provided assembly.
    /// </summary>
    public static CqrsBuilder AddCqrs(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        return new CqrsBuilder(services, assembly, handlerLifetime);
    }

    #endregion

    #region EVENT BUS

    /// <summary>
    /// Adds the Event Bus and event listeners from the calling assembly using the default configuration.
    /// </summary>
    public static EventBusBuilder AddEventBus(this IServiceCollection services, ServiceLifetime listenerLifetime = ServiceLifetime.Transient)
    {
        return services.AddEventBus(Assembly.GetCallingAssembly(), listenerLifetime);
    }

    /// <summary>
    /// Adds the Event Bus and event listeners using the provided options.
    /// </summary>
    public static EventBusBuilder AddEventBus(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime listenerLifetime = ServiceLifetime.Transient)
    {
        return new EventBusBuilder(services, assembly, listenerLifetime);
    }

    #endregion

    #region JOBS

    /// <summary>
    /// Adds the job scheduler and handlers implemented from the calling assembly.
    /// </summary>
    public static JobsBuilder AddJobs(this IServiceCollection services, ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        return services.AddJobs(Assembly.GetCallingAssembly(), handlerLifetime);
    }

    /// <summary>
    /// Adds the job scheduler and handlers from the provided assembly.
    /// </summary>
    public static JobsBuilder AddJobs(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        return new JobsBuilder(services)
            .AddHandlers(assembly, handlerLifetime);
    }

    #endregion

    #region ORCHESTRATION

    /// <summary>
    /// Adds the orchestrator from the calling assembly using the default configuration.
    /// </summary>
    public static OrchestratorBuilder AddOrchestrator(this IServiceCollection services, ServiceLifetime stepLifetime = ServiceLifetime.Transient)
    {
        return services.AddOrchestrator(Assembly.GetCallingAssembly(), stepLifetime);
    }

    /// <summary>
    /// Adds the orchestrator to with the provided options.
    /// </summary>
    public static OrchestratorBuilder AddOrchestrator(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime stepLifetime = ServiceLifetime.Transient)
    {
        return new OrchestratorBuilder(services, assembly, stepLifetime);
    }
    #endregion

    #region REQUESTS

    /// <summary>
    /// Adds requests from the calling assembly using default configuration.
    /// </summary>
    public static RequestsBuilder AddRequests(this IServiceCollection services, ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        return services.AddRequests(Assembly.GetCallingAssembly(), handlerLifetime);
    }

    /// <summary>
    /// Adds requests from the provided assembly.
    /// </summary>
    public static RequestsBuilder AddRequests(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        return new(services, assembly, handlerLifetime);
    }

    #endregion

    #region MISCELLANEOUS

    /// <summary>
    /// Adds all types that implement the generic interface represented from the provided assembly to the service collection.
    /// The types are added as the interface they implement.
    /// </summary>
    public static IServiceCollection TryAddImplementersOfGenericAsImplemented(
        this IServiceCollection services,
        Type genericType,
        Assembly assembly,
        ServiceLifetime lifetime)
    {
        List<ServiceDescriptor> descriptors = [];

        IEnumerable<Type> implementationTypes = assembly.GetTypes()
            .Where(item => item.ImplementsGenericInterface(genericType) && !item.IsAbstract && !item.IsInterface);

        foreach (Type implementationType in implementationTypes)
        {
            IEnumerable<Type> serviceTypes = implementationType.GetInterfaces().Where(i => i.GetGenericTypeDefinition() == genericType);

            foreach (var serviceType in serviceTypes)
            {
                if (implementationType.ShouldRegisterAs(serviceType))
                {
                    ServiceDescriptor descriptor = new(serviceType, implementationType, lifetime);
                    descriptors.Add(descriptor);
                }
            }
        }
        services.TryAddEnumerable(descriptors);

        return services;
    }

    ///<inheritdoc cref="TryAddImplementersOfGenericAsImplemented(IServiceCollection, Type, Assembly, ServiceLifetime)"/>
    public static IServiceCollection TryAddImplementersOfGenericAsImplemented<TService>(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime)
    {
        return services.TryAddImplementersOfGenericAsImplemented(typeof(TService), assembly, lifetime);
    }

    /// <summary>
    /// Adds all types that implement the generic interface represented from the provided assembly to the service collection.
    /// The types are added as the concrete type they are.
    /// </summary>
    public static IServiceCollection TryAddImplementersOfGenericAsSelf(
        this IServiceCollection services,
        Type genericType,
        Assembly assembly,
        ServiceLifetime lifetime)
    {

        IEnumerable<ServiceDescriptor> descriptors = assembly.GetTypes()
            .Where(item => item.ImplementsGenericInterface(genericType) && !item.IsAbstract && !item.IsInterface)
            .ToHashSet()
            .Select(x => new ServiceDescriptor(x, x, lifetime));

        services.TryAddEnumerable(descriptors);

        return services;
    }

    ///<inheritdoc cref="TryAddImplementersOfGenericAsSelf(IServiceCollection, Type, Assembly, ServiceLifetime)"/>
    public static IServiceCollection TryAddImplementersOfGenericAsSelf<TService>(
        this IServiceCollection services, 
        Assembly assembly, 
        ServiceLifetime lifetime)
    {
        return services.TryAddImplementersOfGenericAsSelf(typeof(TService), assembly, lifetime);
    }

    /// <summary>
    /// Adds all types that implement the provided service type from the provided assembly to the service collection.
    /// </summary>
    /// <remarks>
    /// The services are added as multiples of the service type they implement.
    /// </remarks>
    public static IServiceCollection TryAddImplementersAsImplemented(
        this IServiceCollection services,
        Type serviceType, 
        Assembly assembly, 
        ServiceLifetime lifetime)
    {
        IEnumerable<ServiceDescriptor> descriptors = assembly.GetTypes()
            .Where(item => item.IsAssignableTo(serviceType) && !item.IsAbstract && !item.IsInterface && item.ShouldRegisterAs(serviceType))
            .ToHashSet()
            .Select(x => new ServiceDescriptor(serviceType, x, lifetime));
        services.TryAddEnumerable(descriptors);
        return services;
    }

    ///<inheritdoc cref="TryAddImplementersAsImplemented(IServiceCollection, Type, Assembly, ServiceLifetime)"/>
    public static IServiceCollection TryAddImplementersAsImplemented<TService>(this IServiceCollection services, Assembly assembly, ServiceLifetime lifetime)
    {
        return services.TryAddImplementersAsImplemented(typeof(TService), assembly, lifetime);
    }

    /// <summary>
    /// Adds all types that implement the provided service type from the provided assembly to the service collection.
    /// </summary>
    public static IServiceCollection TryAddImplementersAsSelf(
        this IServiceCollection services,
        Type serviceType,
        Assembly assembly,
        ServiceLifetime lifetime)
    {
        IEnumerable<ServiceDescriptor> descriptors = assembly.GetTypes()
            .Where(item => item.IsAssignableTo(serviceType) && !item.IsAbstract && !item.IsInterface)
            .ToHashSet()
            .Select(x => new ServiceDescriptor(x, x, lifetime));
        services.TryAddEnumerable(descriptors);
        return services;
    }

    ///<inheritdoc cref="TryAddImplementersAsSelf(IServiceCollection, Type, Assembly, ServiceLifetime)"/>
    public static IServiceCollection TryAddImplementersAsSelf<TService>(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime)
    {
        return services.TryAddImplementersAsSelf(typeof(TService), assembly, lifetime);
    }

    /// <summary>
    /// Returns true if <paramref name="implementation"/> should be registered
    /// as <paramref name="serviceType"/>, taking into account any
    /// [NotRegistered] attributes on the implementation.
    /// </summary>
    public static bool ShouldRegisterAs(this Type implementation, Type serviceType)
    {
        // grab all [NotRegistered] on the class (don't inherit)
        var exclusions = implementation
            .GetCustomAttributes<NotRegisteredAttribute>(inherit: false);

        // if any attribute has ServiceType == null, we block ALL registrations
        if (exclusions.Any(attr => attr.ServiceType == null))
            return false;

        // otherwise, block only if there's an attribute matching exactly this serviceType
        return !exclusions.Any(attr => attr.ServiceType == serviceType);
    }
    #endregion
}
