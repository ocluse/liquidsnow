using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reflection;

namespace Ocluse.LiquidSnow.Jobs.Internal
{
    internal class JobScheduler : IJobScheduler
    {
        record JobSubscription(IDisposable Handle, IJob Job, CancellationTokenSource CancellationTokenSource) : IDisposable
        {
            public void Dispose()
            {
                Handle.Dispose();
                CancellationTokenSource.Cancel();
            }
        }

        record JobQueueItem(IJob Job, long Tick, CancellationToken CancellationToken);

        private readonly Dictionary<object, JobSubscription> _subscriptions = [];

        private readonly BlockingCollection<JobQueueItem> _jobQueue = [];

        private readonly IServiceProvider _serviceProvider;

        public JobScheduler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Task.Factory.StartNew(HandleQueue, TaskCreationOptions.LongRunning);
        }

        public event EventHandler<JobFailedEventArgs>? JobFailed;

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
                    JobFailed?.Invoke(this, new JobFailedEventArgs(job, ex));
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
                        JobFailed?.Invoke(this, new JobFailedEventArgs(job, ex));
                    }
                }
            }

            if (job is not IRoutineJob)
            {
                _subscriptions.Remove(job.Id);
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
