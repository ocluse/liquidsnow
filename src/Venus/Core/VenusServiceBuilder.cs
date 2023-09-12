using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocluse.LiquidSnow.Venus.Contracts;

namespace Ocluse.LiquidSnow.Venus
{
    /// <summary>
    /// A builder for configuring Venus services.
    /// </summary>
    public class VenusServiceBuilder
    {
        private readonly IServiceCollection _services;

        /// <summary>
        /// The service collection where Venus services are configured.
        /// </summary>
        public IServiceCollection Services => _services;

        internal VenusServiceBuilder(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// Adds a custom <see cref="IVenusResolver"/> removing any existing resolver.
        /// </summary>
        public VenusServiceBuilder AddResolver<T>() where T : class, IVenusResolver
        {
            //Remove existing type:
            Services.RemoveAll(typeof(IVenusResolver));
            Services.AddSingleton<IVenusResolver, T>();

            return this;
        }
    }
}
