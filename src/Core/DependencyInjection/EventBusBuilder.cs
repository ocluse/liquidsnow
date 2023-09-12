using Microsoft.Extensions.DependencyInjection;
using Ocluse.LiquidSnow.Events;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ocluse.LiquidSnow.DependencyInjection
{
    /// <summary>
    /// Builder for adding event handlers to the service collection.
    /// </summary>
    public class EventBusBuilder
    {
        private readonly ServiceLifetime _handlerLifetime;

        /// <summary>
        /// Gets the service collection where the handlers are configured.
        /// </summary>
        public IServiceCollection Services { get; }

        internal EventBusBuilder(ServiceLifetime handlerLifetime, IServiceCollection services)
        {
            _handlerLifetime = handlerLifetime;
            Services = services;
        }

        ///<inheritdoc cref="AddHandlers(IEnumerable{Assembly})"/>
        public EventBusBuilder AddHandlers(params Assembly[] assemblies)
        {
            return AddHandlers(assemblies.AsEnumerable());
        }

        /// <summary>
        /// Adds event handlers from the provided assemblies.
        /// </summary>
        public EventBusBuilder AddHandlers(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                Services.AddImplementers(typeof(IEventHandler<>), assembly, _handlerLifetime, false);
            }
            return this;
        }
    }
}