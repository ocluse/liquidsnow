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
                ServiceDescriptor descriptor = new(serviceType, implementationType, lifetime);
                descriptors.Add(descriptor);
            }
        }
        services.TryAddEnumerable(descriptors);

        return services;
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

    #endregion
}
