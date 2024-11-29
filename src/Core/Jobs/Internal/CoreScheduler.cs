using System.Collections.Concurrent;

namespace Ocluse.LiquidSnow.Jobs.Internal;

internal abstract class CoreScheduler : IJobSubscriptionHandler
{
    private readonly ConcurrentDictionary<object, JobSubscription> _subscriptions = [];

    public abstract Task HandleAsync(JobSubscription jobSubscription);

    public IDisposable Subscribe<T>(T job) where T : IJob
    {
        Cancel(job.Id);
        
        JobSubscription subscription = new(job, this, typeof(T)); 
        
        if(_subscriptions.TryAdd(job.Id, subscription))
        {
            subscription.Disposed += OnSubscriptionDisposed;

            subscription.Activate();

            return subscription;
        }
        else
        {
            throw new InvalidOperationException("Failed to add job: Job with the same ID was added in a different thread!");
        }
    }

    private void OnSubscriptionDisposed(object? sender, JobSubscription e)
    {
        _subscriptions.TryRemove(e.Job.Id, out _);
    }

    public bool Cancel(object id)
    {
        if (_subscriptions.TryGetValue(id, out var subscription))
        {
            subscription.Dispose();
            return true;
        }
        else
        {
            return false;
        }
    }
}
