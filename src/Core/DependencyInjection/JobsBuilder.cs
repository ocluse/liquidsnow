using Microsoft.Extensions.DependencyInjection;
using Ocluse.LiquidSnow.Jobs;
using System.Reflection;

namespace Ocluse.LiquidSnow.DependencyInjection
{
    /// <summary>
    /// Builder for adding jobs to the service collection.
    /// </summary>
    public class JobsBuilder
    {
        private readonly ServiceLifetime _handlerLifetime;

        internal JobsBuilder(ServiceLifetime handlerLifetime, IServiceCollection services)
        {
            _handlerLifetime = handlerLifetime;
            Services = services;
        }

        /// <summary>
        /// Gets the service collection where the handlers are configured.
        /// </summary>
        public IServiceCollection Services { get; }


        /// <inheritdoc cref="AddHandlers(IEnumerable{Assembly})"/>
        public JobsBuilder AddHandlers(params Assembly[] assemblies)
        {
            return AddHandlers(assemblies.AsEnumerable());
        }

        /// <summary>
        /// Adds job handlers from the provided assemblies.
        /// </summary>
        public JobsBuilder AddHandlers(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                Services.AddImplementers(typeof(IJobHandler<>), assembly, _handlerLifetime);
            }
            return this;
        }
    }
}
