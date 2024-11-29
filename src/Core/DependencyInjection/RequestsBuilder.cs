using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocluse.LiquidSnow.Requests;
using Ocluse.LiquidSnow.Requests.Internal;
using System.Reflection;

namespace Ocluse.LiquidSnow.DependencyInjection;

/// <summary>
/// Provides methods for configuring requests in a service collection.
/// </summary>
public class RequestsBuilder
{
    /// <summary>
    /// Gets the service collection where the requests are configured.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Creates a new instance of <see cref="RequestsBuilder"/> and adds essential request services.
    /// </summary>
    public RequestsBuilder(IServiceCollection services)
    {
        Services = services;
        
        AddCore();

    }

    /// <summary>
    /// Creates a new instance of the <see cref="RequestsBuilder"/>, adds essential request services and adds handlers from the provided assembly.
    /// </summary>
    public RequestsBuilder(IServiceCollection services, Assembly assembly, ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        Services = services;

        AddCore();
        AddRequests(assembly, handlerLifetime);
    }

    private void AddCore()
    {
        Services.TryAddTransient<IRequestDispatcher, RequestDispatcher>();
        Services.TryAddSingleton<RequestDescriptorCache>();
    }

    /// <summary>
    /// Adds requests from the provided assembly.
    /// </summary>
    public RequestsBuilder AddRequests(Assembly assembly, ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        Services.TryAddImplementersOfGenericAsImplemented(typeof(IRequestHandler<>), assembly, handlerLifetime);
        Services.TryAddImplementersOfGenericAsImplemented(typeof(IRequestHandler<,>), assembly, handlerLifetime);

        return this;
    }

    /// <summary>
    /// Adds requests from the provided assemblies.
    /// </summary>
    public RequestsBuilder AddRequests(IEnumerable<Assembly> assemblies, ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        foreach (var assembly in assemblies)
        {
            AddRequests(assembly, handlerLifetime);
        }
        return this;
    }
}
