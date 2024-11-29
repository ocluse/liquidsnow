using Microsoft.Extensions.DependencyInjection;

namespace Ocluse.LiquidSnow.Http.Client;

internal class SnowClientBuilder(IServiceCollection services) : ISnowClientBuilder
{
    public IServiceCollection Services { get; } = services;
}
