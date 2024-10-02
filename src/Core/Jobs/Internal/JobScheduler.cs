using Microsoft.Extensions.DependencyInjection;
using Ocluse.LiquidSnow.Events;
using Ocluse.LiquidSnow.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Jobs.Internal;


internal class JobScheduler(IServiceProvider serviceProvider) : CoreScheduler, IJobScheduler
{
    private readonly Dictionary<object, QueueingScheduler> _queueingSchedulers = [];

    private QueueingScheduler GetQueueingScheduler(object queueId)
    {
        return _queueingSchedulers.GetOrAdd(queueId, () => new QueueingScheduler(this));
    }

    private static async Task ExecuteHandler(object? handler, MethodInfo handleMethodInfo, object[] handleMethodArgs)
    {
        if (handler == null)
        {
            return;
        }

        await (Task)handleMethodInfo.Invoke(handler, handleMethodArgs)!;
    }

    public override async Task Execute(IJob job, long tick, CancellationToken cancellationToken)
    {
        Type jobType = job.GetType();

        Type jobHandlerType = typeof(IJobHandler<>).MakeGenericType(jobType);

        Type[] paramTypes = [jobType, typeof(long), typeof(CancellationToken)];

        MethodInfo handleMethodInfo = jobHandlerType.GetMethod("Handle", paramTypes)
            ?? throw new InvalidOperationException("Handle method not found on job handler");

        object[] handleMethodArgs = [job, tick, cancellationToken];

        using IServiceScope scope = serviceProvider.CreateScope();

        IEnumerable<object?> handlers;

        bool executeParallel;

        if (job is IMulticastJob multicastJob)
        {
            handlers = scope.ServiceProvider.GetServices(jobHandlerType);
            executeParallel = multicastJob.ExecuteParallel;
        }
        else
        {
            handlers = [scope.ServiceProvider.GetService(jobHandlerType)];
            executeParallel = false;
        }

        if (executeParallel)
        {
            try
            {
                await Task.WhenAll(handlers.Select(handler =>
                    ExecuteHandler(handler, handleMethodInfo, handleMethodArgs)));
            }
            catch (Exception ex)
            {
                await PublishJobFailedEvent(scope.ServiceProvider, job, ex);
            }
        }
        else
        {
            foreach (object? handler in handlers)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                try
                {
                    await ExecuteHandler(handler, handleMethodInfo, handleMethodArgs);
                }
                catch (Exception ex)
                {
                    await PublishJobFailedEvent(scope.ServiceProvider, job, ex);
                }
            }
        }
    }
    
    private static async Task PublishJobFailedEvent(IServiceProvider scopeServiceProvider, IJob job, Exception exception)
    {
        var eventBus = scopeServiceProvider.GetService<IEventBus>();

        if (eventBus != null)
        {
            await eventBus.Publish(new JobFailedEvent(job, exception), PublishStrategy.Sequential);
        }
        else
        {
            var handlers = scopeServiceProvider.GetServices<IEventHandler<JobFailedEvent>>();

            if (handlers.Any())
            {
                var failedEvent = new JobFailedEvent(job, exception);

                foreach (var handler in handlers)
                {
                    await handler.Handle(failedEvent);
                }
            }
        }
    }

    public IDisposable Schedule(IJob job)
    {
        return Subscribe(job);
    }

    public IDisposable Queue(IQueueJob job)
    {
        var scheduler = GetQueueingScheduler(job.QueueId);
        
        return scheduler.Subscribe(job);
    }

    public bool Cancel(object queueId, object id)
    {
        if(_queueingSchedulers.TryGetValue(queueId, out var scheduler))
        {
            return scheduler.Cancel(id);
        }

        return false;
    }
}
