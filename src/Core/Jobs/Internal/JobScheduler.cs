using Microsoft.Extensions.DependencyInjection;
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

        private readonly Dictionary<object, JobSubscription> _subscriptions;
        private readonly IServiceProvider _serviceProvider;

        public JobScheduler(IServiceProvider serviceProvider)
        {
            _subscriptions = [];
            _serviceProvider = serviceProvider;
        }

        private async void ExecuteJob(IJob job, long tick, CancellationToken cancellationToken)
        {
            Type jobType = job.GetType();

            Type jobHandlerType = typeof(IJobHandler<>).MakeGenericType(jobType);

            Type[] paramTypes = [jobType, typeof(long), typeof(CancellationToken)];

            MethodInfo handleMethodInfo = jobHandlerType.GetMethod("Handle", paramTypes)
                ?? throw new InvalidOperationException("Handle method not found on job handler");

            object[] handleMethodArgs = [job, tick, cancellationToken];

            using IServiceScope scope = _serviceProvider.CreateScope();

            IEnumerable<object?> handlers = scope.ServiceProvider.GetServices(jobHandlerType);

            var handleTasks = new List<TaskCompletionSource<bool>>();

            await Task.WhenAll(handlers.Select(handler =>
                (Task)handleMethodInfo.Invoke(handler, handleMethodArgs)!));

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
