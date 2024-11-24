using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocluse.LiquidSnow.Jobs;
using Ocluse.LiquidSnow.Jobs.Internal;
using System.Reflection;

namespace Ocluse.LiquidSnow.DependencyInjection;

/// <summary>
/// Builder for adding jobs to the service collection.
/// </summary>
public class JobsBuilder
{
    /// <summary>
    /// Creates a new instance of <see cref="JobsBuilder"/> and adds essential Jobs services.
    /// </summary>
    public JobsBuilder(IServiceCollection services)
    {
        Services = services;

        AddCore();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="JobsBuilder"/>, adds essential Jobs services and adds handlers from the provided assembly.
    /// </summary>
    public JobsBuilder(IServiceCollection services, Assembly assembly, ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        Services = services;

        AddCore();
        AddHandlers(assembly, handlerLifetime);
    }

    private void AddCore()
    {
        Services.TryAddSingleton<IJobScheduler, JobScheduler>();
        Services.TryAddSingleton<JobDescriptorCache>();
        Services.TryAddTransient<IJobDispatcher, JobDispatcher>();
    }

    /// <summary>
    /// Gets the service collection where the handlers are configured.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Adds job handlers from the provided assembly.
    /// </summary>
    public JobsBuilder AddHandlers(Assembly assembly, ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {
        Services.TryAddImplementersOfGenericAsImplemented(typeof(IJobHandler<>), assembly, handlerLifetime);
        return this;
    }

    /// <summary>
    /// Adds job handlers from the provided assemblies.
    /// </summary>
    public JobsBuilder AddHandlers(IEnumerable<Assembly> assemblies, ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
    {        
        foreach (var assembly in assemblies)
        {
            AddHandlers(assembly, handlerLifetime);
        }
        return this;
    }
}
