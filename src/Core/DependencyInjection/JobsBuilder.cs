using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocluse.LiquidSnow.Jobs;
using Ocluse.LiquidSnow.Jobs.Internal;
using Ocluse.LiquidSnow.Jobs.Persistence;
using Microsoft.Extensions.Hosting;
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
        Services.TryAddSingleton<JobsOptions>();
        Services.TryAddSingleton<IJobStore, InMemoryJobStore>();
        Services.TryAddSingleton<IJobSerializer, JsonJobSerializer>();
        Services.TryAddSingleton<IJobKeySerializer, DefaultJobKeySerializer>();
        Services.TryAddSingleton(TimeProvider.System);
        Services.TryAddSingleton<JobScheduler>();
        Services.TryAddSingleton<IJobScheduler>(provider => provider.GetRequiredService<JobScheduler>());
        Services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, JobSchedulerHostedService>());
        Services.TryAddSingleton<JobDescriptorCache>();
        Services.TryAddTransient<IJobDispatcher, JobDispatcher>();
    }

    /// <summary>
    /// Configures Jobs options.
    /// </summary>
    public JobsBuilder Configure(Action<JobsOptions> configure)
    {
        var options = new JobsOptions();
        configure(options);
        Services.AddSingleton(options);
        return this;
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

        IEnumerable<Type> jobTypes = assembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .SelectMany(type => type.GetInterfaces())
            .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IJobHandler<>))
            .Select(type => type.GetGenericArguments()[0])
            .Distinct();

        foreach (Type jobType in jobTypes)
        {
            string typeName = jobType.FullName
                ?? throw new InvalidOperationException("Durable job types must have a full name.");
            Services.AddSingleton(new JobTypeRegistration(jobType, typeName));
        }

        return this;
    }

    /// <summary>
    /// Registers a stable serialized name for a durable job type.
    /// </summary>
    public JobsBuilder AddJob<T>(string typeName) where T : IJob
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);
        Services.AddSingleton(new JobTypeRegistration(typeof(T), typeName));
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
