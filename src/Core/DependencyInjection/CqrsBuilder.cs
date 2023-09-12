using Microsoft.Extensions.DependencyInjection;
using Ocluse.LiquidSnow.Cqrs;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ocluse.LiquidSnow.DependencyInjection
{
    /// <summary>
    ///  Builder for adding CQRS handlers to the service collection.
    ///  </summary>
    public class CqrsBuilder
    {
        private readonly ServiceLifetime _handlerLifetime;

        /// <summary>
        /// Gets the service collection where the handlers are configured.
        /// </summary>
        public IServiceCollection Services { get; }

        internal CqrsBuilder(ServiceLifetime handlerLifetime, IServiceCollection services)
        {
            _handlerLifetime = handlerLifetime;
            Services = services;
        }

        /// <inheritdoc cref="AddHandlers(IEnumerable{Assembly})"/>
        public CqrsBuilder AddHandlers(params Assembly[] assemblies)
        {
            return AddHandlers(assemblies.AsEnumerable());
        }

        /// <summary>
        /// Adds CQRS handlers from the provided assemblies.
        /// </summary>
        public CqrsBuilder AddHandlers(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                Services.AddImplementers(typeof(IQueryHandler<,>), assembly, _handlerLifetime);
                Services.AddImplementers(typeof(ICommandHandler<,>), assembly, _handlerLifetime);
                Services.AddImplementers(typeof(IPreCommandExecutionHandler<,>), assembly, _handlerLifetime);
                Services.AddImplementers(typeof(IPostCommandExecutionHandler<,>), assembly, _handlerLifetime);
                Services.AddImplementers(typeof(IPreQueryExecutionHandler<,>), assembly, _handlerLifetime);
                Services.AddImplementers(typeof(IPostQueryExecutionHandler<,>), assembly, _handlerLifetime);
            }
            return this;
        }
    }
}