using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Ocluse.LiquidSnow.Events;

namespace Ocluse.LiquidSnow.Jobs.Internal;

internal class JobDispatcher(JobDescriptorCache descriptorCache, IServiceProvider serviceProvider) : IJobDispatcher
{
    private static async Task ExecuteHandler(object? handler, MethodInfo handleMethodInfo, object[] handleMethodArgs)
    {
        if (handler == null)
        {
            return;
        }

        await (Task)handleMethodInfo.Invoke(handler, handleMethodArgs)!;
    }

    private void PublishJobFailedEvent(object job, long tick, Exception exception)
    {
        IEventBus? eventBus = serviceProvider.GetService<IEventBus>();

        if (eventBus != null)
        {
            eventBus.Publish(new JobFailedEvent((IJob)job, tick, exception));
        }
        else
        {
            _ = Task.Run(async () =>
            {
                using IServiceScope scope = serviceProvider.CreateScope();

                IEnumerable<IEventListener<JobFailedEvent>> handlers = scope.ServiceProvider.GetServices<IEventListener<JobFailedEvent>>();

                if (handlers.Any())
                {
                    var failedEvent = new JobFailedEvent((IJob)job, tick, exception);

                    foreach (var handler in handlers)
                    {
                        await handler.HandleAsync(failedEvent);
                    }
                }
            });
        }
    }

    public async Task DispatchAsync<T>(T job, long tick, bool throwExceptions, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(job, nameof(job));
        await DispatchAsync(job, typeof(T), tick, throwExceptions, cancellationToken);
    }

    public async Task DispatchAsync(object job, Type jobType, long tick, bool throwExceptions, CancellationToken cancellationToken = default)
    {
        JobDescriptor descriptor = descriptorCache.GetDescriptor(jobType);

        object[] handleMethodArgs = [job, tick, cancellationToken];

        if (job is IMulticastJob multicastJob)
        {
            IEnumerable<object?> handlers = serviceProvider.GetServices(descriptor.HandlerType);

            try
            {
                if (multicastJob.ExecuteParallel)
                {
                    await Task.WhenAll(handlers.Select(handler =>
                    ExecuteHandler(handler, descriptor.HandleMethodInfo, handleMethodArgs)));
                }
                else
                {
                    foreach (object? handler in handlers)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        await ExecuteHandler(handler, descriptor.HandleMethodInfo, handleMethodArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                if (throwExceptions)
                {
                    throw;
                }
                else
                {
                    PublishJobFailedEvent(job, tick, ex);
                }
            }
        }
        else
        {
            object? handler = serviceProvider.GetService(descriptor.HandlerType);

            try
            {
                await ExecuteHandler(handler, descriptor.HandleMethodInfo, handleMethodArgs);
            }
            catch (Exception ex)
            {
                if (throwExceptions)
                {
                    throw;
                }
                else
                {
                    PublishJobFailedEvent(job,tick, ex);
                }
            }
        }
    }
}
