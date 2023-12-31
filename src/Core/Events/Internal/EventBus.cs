﻿using Microsoft.Extensions.DependencyInjection;
using Ocluse.LiquidSnow.Extensions;
using System.Reflection;

namespace Ocluse.LiquidSnow.Events.Internal
{
    internal class EventBus : IEventBus
    {
        private readonly PublishStrategy _strategy;
        private readonly IServiceProvider _serviceProvider;
        public EventBus(EventBusOptions options, IServiceProvider serviceProvider)
        {
            _strategy = options.PublishStrategy;
            _serviceProvider = serviceProvider;
        }

        private static async Task ExecuteHandler(object? handler, MethodInfo handleMethodInfo, object[] handleMethodArgs)
        {
            if (handler == null)
            {
                return;
            }

            await (Task)handleMethodInfo.Invoke(handler, handleMethodArgs)!;
        }

        public async Task Publish(IEvent ev, PublishStrategy? strategy = null, CancellationToken cancellationToken = default)
        {
            strategy ??= _strategy;

            Type eventType = ev.GetType();

            Type eventHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);

            Type[] paramTypes = [eventType, typeof(CancellationToken)];

            MethodInfo handleMethodInfo = eventHandlerType.GetMethod("Handle", paramTypes) ?? throw new InvalidOperationException("Handle method not found on event handler");

            object[] handleMethodArgs = [ev, cancellationToken];

            IEnumerable<object?> handlers = _serviceProvider
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
    }
}
