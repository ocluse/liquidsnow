using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Ocluse.LiquidSnow.Cqrs.Internal;
using Ocluse.LiquidSnow.Cqrs;
using Ocluse.LiquidSnow.Events;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Orchestrations.Internal;
using Ocluse.LiquidSnow.Orchestrations;
using Ocluse.LiquidSnow.Events.Internal;
using Ocluse.LiquidSnow.Jobs;
using Ocluse.LiquidSnow.Jobs.Internal;

namespace Ocluse.LiquidSnow.DependencyInjection
{
    /// <summary>
    /// Extension methods to add CQRS, Event, and Orchestration handlers and dispatchers to a DI container.
    /// </summary>
    public static class BuilderExtensions
    {
        #region CQRS

        /// <summary>
        /// Adds CQRS from the calling assembly using default configuration.
        /// </summary>
        public static CqrsBuilder AddCqrs(this IServiceCollection services)
        {
            return services.AddCqrs(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Adds CQRS from the provided options.
        /// </summary>
        public static CqrsBuilder AddCqrs(
            this IServiceCollection services,
            Assembly assembly,
            ServiceLifetime dispatcherLifetime = ServiceLifetime.Transient,
            ServiceLifetime handlerLifetime = ServiceLifetime.Scoped)
        {
            ServiceDescriptor commandsDescriptor
                = new(typeof(ICommandDispatcher), typeof(CommandDispatcher), dispatcherLifetime);

            ServiceDescriptor queriesDescriptor
                = new(typeof(IQueryDispatcher), typeof(QueryDispatcher), dispatcherLifetime);

            services.TryAdd(commandsDescriptor);
            services.TryAdd(queriesDescriptor);

            CqrsBuilder builder = new(handlerLifetime, services);

            return builder.AddHandlers(assembly);
        }

        #endregion

        #region JOBS

        /// <summary>
        /// Adds jobs from the calling assembly using default configuration.
        /// </summary>
        public static JobsBuilder AddJobs(this IServiceCollection services)
        {
            return services.AddJobs(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Adds jobs from the provided options.
        /// </summary>
        public static JobsBuilder AddJobs(
            this IServiceCollection services,
            Assembly assembly,
            ServiceLifetime handlerLifetime = ServiceLifetime.Transient)
        {
            ServiceDescriptor schedulerDescriptor
                = new(typeof(IJobScheduler), typeof(JobScheduler), ServiceLifetime.Singleton);

            services.TryAdd(schedulerDescriptor);

            JobsBuilder builder = new(handlerLifetime, services);

            return builder.AddHandlers(assembly);
        }

        #endregion

        #region EVENT BUS

        /// <summary>
        /// Adds the Event Bus and event handlers from the calling assembly using the default configuration
        /// </summary>
        public static EventBusBuilder AddEventBus(this IServiceCollection services)
        {
            return services.AddEventBus(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Adds the Event Bus and event handlers using the provided options.
        /// </summary>
        public static EventBusBuilder AddEventBus(
            this IServiceCollection services,
            Assembly assembly,
            ServiceLifetime busLifetime = ServiceLifetime.Singleton,
            ServiceLifetime handlerLifetime = ServiceLifetime.Transient,
            PublishStrategy publishStrategy = PublishStrategy.Sequential)
        {
            var options = new EventBusOptions()
            {
                PublishStrategy = publishStrategy,
                BusLifetime = busLifetime,
                HandlerLifetime = handlerLifetime
            };

            services.TryAddSingleton(options);

            ServiceDescriptor busDescriptor
                = new(typeof(IEventBus), typeof(EventBus), busLifetime);

            services.TryAdd(busDescriptor);


            EventBusBuilder builder = new(handlerLifetime, services);

            return builder.AddHandlers(assembly);
        }

        #endregion

        #region ORCHESTRATION

        /// <summary>
        /// Adds the orchestrator from the calling assembly using the default configuration.
        /// </summary>
        public static OrchestratorBuilder AddOrchestrator(this IServiceCollection services)
        {
            return services.AddOrchestrator(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Adds the orchestrator to with the provided options.
        /// </summary>
        public static OrchestratorBuilder AddOrchestrator(
            this IServiceCollection services,
            Assembly assembly,
            ServiceLifetime orchestratorLifetime = ServiceLifetime.Scoped,
            ServiceLifetime stepLifetime = ServiceLifetime.Transient)
        {

            ServiceDescriptor orchestratorDescriptor = new(typeof(IOrchestrator), typeof(Orchestrator), orchestratorLifetime);

            services.TryAdd(orchestratorDescriptor);

            OrchestratorBuilder builder = new(stepLifetime, services);

            return builder.AddSteps(assembly);
        }
        #endregion

        #region MISCELLANEOUS
        /// <summary>
        /// Adds all types that implement the provided interface type from the provided assembly to the service collection.
        /// </summary>
        public static IServiceCollection AddImplementers(
            this IServiceCollection services,
            Type type,
            Assembly assembly,
            ServiceLifetime lifetime,
            bool doNotAddDuplicates = true)
        {
            List<ServiceDescriptor> descriptors = [];
            assembly.GetTypes()
                .Where(item => item.GetInterfaces()
                .Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == type) && !item.IsAbstract && !item.IsInterface)
                .ToList()
                .ForEach(assignedType =>
                {
                    var serviceTypes = assignedType.GetInterfaces().Where(i => i.GetGenericTypeDefinition() == type);

                    foreach (var serviceType in serviceTypes)
                    {
                        ServiceDescriptor descriptor = new(serviceType, assignedType, lifetime);
                        descriptors.Add(descriptor);
                    }
                });

            if (doNotAddDuplicates)
            {
                services.TryAdd(descriptors);
            }
            else
            {
                services.AddRange(descriptors);
            }

            return services;
        }

        /// <summary>
        /// Adds all types that implement the provided interface type from the provided assembly to the service collection.
        /// The types are added as their concrete selves.
        /// </summary>
        public static IServiceCollection AddImplementersAsSelf(
            this IServiceCollection services,
            Type type,
            Assembly assembly,
            ServiceLifetime lifetime,
            bool doNotAddDuplicates = true)
        {
            List<ServiceDescriptor> descriptors = [];
            assembly.GetTypes()
                .Where(item => item.GetInterfaces()
                .Where(i => i.IsGenericType).Any(i => i.GetGenericTypeDefinition() == type) && !item.IsAbstract && !item.IsInterface)
                .ToList()
                .ForEach(assignedType =>
                {
                    var serviceTypes = assignedType.GetInterfaces().Where(i => i.GetGenericTypeDefinition() == type);

                    foreach (var serviceType in serviceTypes)
                    {
                        ServiceDescriptor descriptor = new(assignedType, assignedType, lifetime);
                        descriptors.Add(descriptor);
                    }
                });

            if (doNotAddDuplicates)
            {
                services.TryAdd(descriptors);
            }
            else
            {
                services.AddRange(descriptors);
            }

            return services;
        }

        #endregion
    }
}
