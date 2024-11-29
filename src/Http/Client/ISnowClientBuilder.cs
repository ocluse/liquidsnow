using Microsoft.Extensions.DependencyInjection;

namespace Ocluse.LiquidSnow.Http.Client;

/// <summary>
/// A builder for adding Snow clients to the service collection
/// </summary>
public interface ISnowClientBuilder
{
    /// <summary>
    /// The service collection where the Snow clients are added
    /// </summary>
    IServiceCollection Services { get; }
}
