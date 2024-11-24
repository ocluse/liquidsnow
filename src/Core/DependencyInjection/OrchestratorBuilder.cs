using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Ocluse.LiquidSnow.Orchestrations;
using Ocluse.LiquidSnow.Orchestrations.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ocluse.LiquidSnow.DependencyInjection;

/// <summary>
/// Provides methods for configuring orchestrations in a service collection.
/// </summary>
public class OrchestratorBuilder
{
    /// <summary>
    /// Gets the service collection where the orchestration steps are configured.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Creates a new instance of <see cref="OrchestratorBuilder"/> and adds essential orchestrator services.
    /// </summary>
    public OrchestratorBuilder(IServiceCollection services)
    {
        Services = services;
        AddCore();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="OrchestratorBuilder"/>, adds essential orchestrator services and adds steps from the provided assembly.
    /// </summary>
    public OrchestratorBuilder(IServiceCollection services, Assembly assembly, ServiceLifetime stepLifetime = ServiceLifetime.Transient)
    {
        Services = services;
        AddCore();
        AddSteps(assembly, stepLifetime);
    }

    private void AddCore()
    {
        Services.TryAddTransient<IOrchestrator, Orchestrator>();
        Services.TryAddSingleton<OrchestrationDescriptorCache>();
    }

    /// <summary>
    /// Adds orchestration steps from the provided assembly.
    /// </summary>
    public OrchestratorBuilder AddSteps(Assembly assembly, ServiceLifetime stepLifetime = ServiceLifetime.Transient)
    {
        Services.TryAddImplementersOfGenericAsImplemented(typeof(IOrchestrationStep<,>), assembly, stepLifetime);
        Services.TryAddImplementersOfGenericAsImplemented(typeof(IOrchestrationPreprocessor<,>), assembly, stepLifetime);
        Services.TryAddImplementersOfGenericAsImplemented(typeof(IOrchestrationPostprocessor<,>), assembly, stepLifetime);

        return this;
    }

    /// <summary>
    /// Adds orchestration steps from the provided assemblies.
    /// </summary>
    public OrchestratorBuilder AddSteps(IEnumerable<Assembly> assemblies, ServiceLifetime stepLifetime = ServiceLifetime.Transient)
    {
        foreach (var assembly in assemblies)
        {
            AddSteps(assembly, stepLifetime);
        }
        return this;
    }
}
