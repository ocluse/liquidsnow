using Microsoft.Extensions.DependencyInjection;
using System.Reactive.Linq;
using System.Reflection;

namespace Ocluse.LiquidSnow.Jobs.Internal
{
    internal class JobScheduler(IServiceProvider _serviceProvider) : IJobScheduler
    {
        record JobSubscription(IDisposable Handle, IJob Job, CancellationTokenSource CancellationTokenSource) : IDisposable
        {
            public void Dispose()
            {
                Handle.Dispose();
                CancellationTokenSource.Cancel();
            }
        }

        private readonly Dictionary<object, JobSubscription> _subscriptions = [];

        public event EventHandler<JobFailedEventArgs>? JobFailed;

        private static async Task ExecuteHandler(object? handler, MethodInfo handleMethodInfo, object[] handleMethodArgs)
        {
            if (handler == null)
            {
                return;
            }

            await (Task)handleMethodInfo.Invoke(handler, handleMethodArgs)!;
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
