using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocluse.LiquidSnow.Cqrs;
using Ocluse.LiquidSnow.Cqrs.Internal;

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

    private void AddCore()
    {
        Services.TryAddTransient<ICommandDispatcher, CommandDispatcher>();
        Services.TryAddTransient<IQueryDispatcher, QueryDispatcher>();
        Services.TryAddTransient<CoreDispatcher>();
        Services.TryAddEnumerable(ServiceDescriptor.Singleton<ICqrsDispatchContributor, EmptyCqrsDispatchContributor>());
    }
}
