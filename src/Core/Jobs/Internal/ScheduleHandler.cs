using Microsoft.Extensions.DependencyInjection;
using Ocluse.LiquidSnow.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Jobs.Internal
{
    internal class ScheduleHandler
    {
        private readonly Dictionary<object, JobSubscription> _subscriptions = [];

        private readonly BlockingCollection<JobQueueItem> _jobQueue = [];

        private readonly IServiceProvider _serviceProvider;

        public ScheduleHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Task.Factory.StartNew(HandleQueue, TaskCreationOptions.LongRunning);
        }

        private async void HandleQueue()
        {
            while (true)
            {
                var item = _jobQueue.Take();

                if (item.CancellationToken.IsCancellationRequested)
                {
                    continue;
                }
                else
                {
                    await ExecuteJob(item.Job, item.Tick, item.CancellationToken);
                }
            }
        }

        private static async Task ExecuteHandler(object? handler, MethodInfo handleMethodInfo, object[] handleMethodArgs)
        {
            if (handler == null)
            {
                return;
            }

            await (Task)handleMethodInfo.Invoke(handler, handleMethodArgs)!;
        }

        private async Task ExecuteJob(IJob job, long tick, CancellationToken cancellationToken)
        {
            Type jobType = job.GetType();

            Type jobHandlerType = typeof(IJobHandler<>).MakeGenericType(jobType);

            Type[] paramTypes = [jobType, typeof(long), typeof(CancellationToken)];

            MethodInfo handleMethodInfo = jobHandlerType.GetMethod("Handle", paramTypes)
                ?? throw new InvalidOperationException("Handle method not found on job handler");

            object[] handleMethodArgs = [job, tick, cancellationToken];

            using IServiceScope scope = _serviceProvider.CreateScope();

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
                    cancellationToken.ThrowIfCancellationRequested();

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

            if (job is not IRoutineJob)
            {
                _subscriptions.Remove(job.Id);
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

        private void QueueJob(IJob job, long tick, CancellationToken cancellationToken)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                JobQueueItem item = new(job, tick, cancellationToken);
                _jobQueue.Add(item, default);
            }
        }

        private JobSubscription Subscribe(IJob job, Action<IJob, long, CancellationToken> handler)
        {
            IDisposable handle;
            CancellationTokenSource tokenSource = new();

            if (job is IRoutineJob routineJob)
            {
                handle = Observable
                    .Timer(routineJob.Start, routineJob.Interval)
                    .Subscribe(tick => handler(job, tick, tokenSource.Token));
            }
            else
            {
                handle = Observable
                    .Timer(job.Start)
                    .Subscribe(tick => handler(job, tick, tokenSource.Token));
            }

            JobSubscription subscription = new(handle, job, tokenSource);

            //Cancel any jobs with the same ID
            Cancel(job.Id);

            _subscriptions.Add(job.Id, subscription);
            return subscription;
        }

        public IDisposable Schedule(IJob job)
        {
            return Subscribe(job, async (job, tick, cancellationToken) => await ExecuteJob(job, tick, cancellationToken));
        }

        public IDisposable Queue(IJob job)
        {
            return Subscribe(job, QueueJob);
        }

        public bool Cancel(object id)
        {
            if (_subscriptions.TryGetValue(id, out var subscription))
            {
                subscription.Dispose();
                _subscriptions.Remove(id);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
