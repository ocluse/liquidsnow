using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocluse.LiquidSnow.Cqrs;
using Ocluse.LiquidSnow.Cqrs.Internal;
using System.Reflection;

namespace Ocluse.LiquidSnow.DependencyInjection;

/// <summary>
///  Provides methods for configuring CQRS in a service collection.
///  </summary>
public class CqrsBuilder
{
    /// <summary>
    /// Gets the service collection where CQRS is configured.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Creates a new instance of <see cref="CqrsBuilder"/> and adds essential CQRS services.
    /// </summary>
    public CqrsBuilder(IServiceCollection services)
    {
        Services = services;
        AddCore();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="CqrsBuilder"/>, adds essential CQRS services and adds handlers from the provided assembly.
    /// </summary>
    public CqrsBuilder(IServiceCollection services, Assembly assembly, ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        Services = services;
        AddCore();
        AddHandlers(assembly, handlerLifetime);
    }

    private void AddCore()
    {
        Services.TryAddTransient<ICommandDispatcher, CommandDispatcher>();
        Services.TryAddTransient<IQueryDispatcher, QueryDispatcher>();
        Services.TryAddTransient<CoreDispatcher>();
        Services.TryAddSingleton<ExecutionDescriptorCache>();
    }

    /// <summary>
    /// Adds CQRS handlers from the provided assembly.
    /// </summary>
    public CqrsBuilder AddHandlers(Assembly assembly, ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        Services.TryAddImplementersOfGenericAsImplemented(typeof(IQueryHandler<,>), assembly, handlerLifetime);
        Services.TryAddImplementersOfGenericAsImplemented(typeof(ICommandHandler<,>), assembly, handlerLifetime);
        Services.TryAddImplementersOfGenericAsImplemented(typeof(ICommandPreprocessor<,>), assembly, handlerLifetime);
        Services.TryAddImplementersOfGenericAsImplemented(typeof(ICommandPostprocessor<,>), assembly, handlerLifetime);
        Services.TryAddImplementersOfGenericAsImplemented(typeof(IQueryPreprocessor<,>), assembly, handlerLifetime);
        Services.TryAddImplementersOfGenericAsImplemented(typeof(IQueryPostprocessor<,>), assembly, handlerLifetime);
        return this;
    }

    /// <summary>
    /// Adds CQRS handlers from the provided assemblies.
    /// </summary>
    public CqrsBuilder AddHandlers(IEnumerable<Assembly> assemblies, ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        foreach (var assembly in assemblies)
        {
            AddHandlers(assembly, handlerLifetime);
        }
        return this;
    }

    ///<inheritdoc cref="AddHandler(Type, ServiceLifetime)"/>
    public CqrsBuilder AddHandler<T>(ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        return AddHandler(typeof(T), serviceLifetime);
    }

    /// <summary>
    /// Adds the specified command/query handler.
    /// </summary>
    /// <remarks>
    /// If the specified type implements multiple handler interfaces, it will be registered for each.
    /// </remarks>
    public CqrsBuilder AddHandler(Type type, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {

        IEnumerable<ServiceDescriptor> descriptors = type
            .GetInterfaces()
            .Where(i => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) || i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
            .Select(i => new ServiceDescriptor(i, type, serviceLifetime));

        Services.TryAdd(descriptors);

        return this;
    }
}