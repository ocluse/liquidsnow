using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Ocluse.LiquidSnow.Events;

namespace Ocluse.LiquidSnow.Jobs.Internal;

internal class JobDispatcher(JobDescriptorCache descriptorCache, IServiceProvider serviceProvider) : IJobDispatcher
{
    private record MulticastExecutionData(object Handler, JobDescriptor Descriptor);

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
        JobDescriptor[] chain = descriptorCache.GetPolymorphicChain(jobType);

        object[] handleMethodArgs = [job, tick, cancellationToken];

        if (job is IMulticastJob multicastJob)
        {
            List<MulticastExecutionData> executionList = [];

            foreach (var descriptor in chain)
            {
                var handlers = serviceProvider.GetServices(descriptor.HandlerType);

                foreach (var handler in handlers)
                {
                    if (handler != null)
                        executionList.Add(new MulticastExecutionData(handler, descriptor));
                }
            }

            try
            {
                if (multicastJob.ExecuteParallel)
                {
                    await Task.WhenAll(executionList.Select(info =>
                    ExecuteHandler(info.Handler, info.Descriptor.HandleMethodInfo, handleMethodArgs)));
                }
                else
                {
                    foreach (var info in executionList)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        await ExecuteHandler(info.Handler, info.Descriptor.HandleMethodInfo, handleMethodArgs);
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
            (object? handler, JobDescriptor? descriptor) = ResolveHandler(chain, serviceProvider);

            if (descriptor != null && handler != null)
            {
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
                        PublishJobFailedEvent(job, tick, ex);
                    }
                }
            }
        }
    }

    private static (object?, JobDescriptor?) ResolveHandler(JobDescriptor[] chain, IServiceProvider serviceProvider)
    {
        foreach (JobDescriptor descriptor in chain)
        {
            object? handler = serviceProvider.GetService(descriptor.HandlerType);
            if (handler != null)
                return (handler, descriptor);
        }
        return (null, null);
    }
}
