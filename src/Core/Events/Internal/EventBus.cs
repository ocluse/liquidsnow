using Microsoft.Extensions.DependencyInjection;
using Ocluse.LiquidSnow.Extensions;
using System.Reflection;

namespace Ocluse.LiquidSnow.Events.Internal
{
    internal class EventBus(EventBusOptions options, IServiceProvider serviceProvider) : IEventBus
    {
        private readonly PublishStrategy _strategy = options.PublishStrategy;
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ServiceLifetime _lifeTime = options.BusLifetime;

        private static async Task ExecuteHandler(object? handler, MethodInfo handleMethodInfo, object[] handleMethodArgs)
        {
            if (handler == null)
            {
                return;
            }

            await (Task)handleMethodInfo.Invoke(handler, handleMethodArgs)!;
        }

        private static async Task PublishCore(IServiceProvider serviceProvider, IEvent ev, PublishStrategy strategy, CancellationToken cancellationToken)
        {
            Type eventType = ev.GetType();

            Type eventHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

            Type[] paramTypes = [eventType, typeof(CancellationToken)];

            MethodInfo handleMethodInfo = eventHandlerType.GetMethod("Handle", paramTypes) ?? throw new InvalidOperationException("Handle method not found on event handler");

            object[] handleMethodArgs = [ev, cancellationToken];

            IEnumerable<object?> handlers = serviceProvider
                .GetServices(eventHandlerType);

            if (strategy == PublishStrategy.Sequential)
            {
                List<Exception> exceptions = [];

                foreach (object? handler in handlers)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        await ExecuteHandler(handler, handleMethodInfo, handleMethodArgs);
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }

                if (exceptions.Count > 1)
                {
                    throw new AggregateException(exceptions);
                }
            }
            else if (strategy == PublishStrategy.FireAndForget)
            {
                foreach (object? handler in handlers)
                {
                    _ = ExecuteHandler(handler, handleMethodInfo, handleMethodArgs);
                }
            }
            else if (strategy == PublishStrategy.Parallel)
            {
                await Task.WhenAll(handlers.Select(
                    handler => ExecuteHandler(handler, handleMethodInfo, handleMethodArgs)));
            }
            else if (strategy == PublishStrategy.FireAndForgetAfterFirst)
            {
                await Task.WhenAny(handlers.Select(
                    handler => ExecuteHandler(handler, handleMethodInfo, handleMethodArgs)))
                    .WithAggregatedExceptions();
            }
        }

        public async Task Publish(IEvent ev, PublishStrategy? strategy = null, CancellationToken cancellationToken = default)
        {
            IServiceProvider serviceProvider = _serviceProvider;

            if(_lifeTime is ServiceLifetime.Singleton)
            {
                //create a scope
                using IServiceScope scope = serviceProvider.CreateScope();
                serviceProvider = scope.ServiceProvider;
            }

            await PublishCore(serviceProvider, ev, strategy ?? _strategy, cancellationToken);
        }
    }
}
