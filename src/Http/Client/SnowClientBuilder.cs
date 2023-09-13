using Microsoft.Extensions.DependencyInjection;

namespace Ocluse.LiquidSnow.Http.Client
{
    internal class SnowClientBuilder : ISnowClientBuilder
    {
        public IServiceCollection Services { get; }
        public SnowClientBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
