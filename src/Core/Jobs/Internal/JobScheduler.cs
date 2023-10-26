using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Jobs.Internal
{
    internal class JobScheduler : IJobScheduler
    {
        readonly struct JobSubscription : IDisposable
        {
            public JobSubscription(IDisposable handle, IJob job, CancellationTokenSource cancellationTokenSource)
            {
                Handle = handle;
                Job = job;
                CancellationTokenSource = cancellationTokenSource;
            }

            public IDisposable Handle { get; }

            public IJob Job { get; }
            
            public CancellationTokenSource CancellationTokenSource { get; }

            public readonly void Dispose()
            {
                Handle.Dispose();
                CancellationTokenSource.Cancel();
            }
        }

        private readonly Dictionary<object, JobSubscription> _subscriptions;
        private readonly IServiceProvider _serviceProvider;

        public JobScheduler(IServiceProvider serviceProvider)
        {
            _subscriptions = new Dictionary<object, JobSubscription>();
            _serviceProvider = serviceProvider;
        }

        private async void ExecuteJob(IJob job, long tick, CancellationToken cancellationToken)
        {
            Type jobType = job.GetType();

            Type jobHandlerType = typeof(IJobHandler<>).MakeGenericType(jobType);

            Type[] paramTypes = new Type[] { jobType, typeof(long), typeof(CancellationToken) };

            MethodInfo handleMethodInfo = jobHandlerType.GetMethod("Handle", paramTypes)
                ?? throw new InvalidOperationException("Handle method not found on job handler");

            object[] handleMethodArgs = new object[] { job, tick, cancellationToken };

            using IServiceScope scope = _serviceProvider.CreateScope();

            IEnumerable<object?> handlers = scope.ServiceProvider.GetServices(jobHandlerType);

            var handleTasks = new List<TaskCompletionSource<bool>>();

            await Task.WhenAll(handlers.Select(handler =>
                (Task)handleMethodInfo.Invoke(handler, handleMethodArgs)));

            if (!typeof(IRoutineJob).IsAssignableFrom(job.GetType()))
            {
                _subscriptions.Remove(job.Id);
            }
        }

        public IDisposable Schedule(IJob job)
        {
            IDisposable handle;
            CancellationTokenSource tokenSource = new();

            if (job is IRoutineJob routineJob)
            {
                handle = Observable
                    .Timer(routineJob.Start, routineJob.Interval)
                    .Subscribe(tick => ExecuteJob(job, tick, tokenSource.Token));
            }
            else
            {
                handle = Observable
                    .Timer(job.Start)
                    .Subscribe(tick => ExecuteJob(job, tick, tokenSource.Token));
            }

            JobSubscription subscription = new(handle, job, tokenSource);
            _subscriptions.Add(job.Id, subscription);
            return subscription;
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
