using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Ocluse.LiquidSnow.Jobs.Internal;


internal class JobScheduler(IServiceProvider serviceProvider) : CoreScheduler, IJobScheduler
{
    private readonly ConcurrentDictionary<object, QueueingScheduler> _queueingSchedulers = [];

    private QueueingScheduler GetQueueingScheduler(object queueId)
    {
        return _queueingSchedulers.GetOrAdd(queueId, (_) => new QueueingScheduler(this));
    }

    public override async Task HandleAsync(JobSubscription jobSubscription)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        IJobDispatcher dispatcher = scope.ServiceProvider.GetRequiredService<IJobDispatcher>();

        await dispatcher.DispatchAsync(jobSubscription.Job, jobSubscription.JobType, jobSubscription.CurrentTick, false, jobSubscription.CancellationToken);
    }

    public IDisposable Schedule<T>(T job) where T : IJob
    {
        return Subscribe(job);
    }

    public IDisposable Queue<T>(T job) where T : IQueueJob
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
