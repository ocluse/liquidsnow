namespace Ocluse.LiquidSnow.Jobs.Internal;

internal abstract class CoreScheduler : IJobExecutor
{
    private readonly Dictionary<object, JobSubscription> _subscriptions = [];

    public abstract Task Execute(IJob job, long tick, CancellationToken cancellationToken);

    public IDisposable Subscribe(IJob job)
    {
        //Cancel any jobs with the same ID
        Cancel(job.Id);
        JobSubscription subscription = new(job, this);
        _subscriptions.Add(job.Id, subscription);

        subscription.Disposed += OnSubscriptionDisposed;

        subscription.Activate();
        return subscription;
    }

    private void OnSubscriptionDisposed(object? sender, JobSubscription e)
    {
        _subscriptions.Remove(e.Job.Id);
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
