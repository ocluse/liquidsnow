using System.Collections.Concurrent;

namespace Ocluse.LiquidSnow.Jobs.Internal;

internal class QueueingScheduler : CoreScheduler
{
    internal record QueuedItem(JobSubscription Subscription)
    {
        public TaskCompletionSource CompletionSource { get; } = new();
    }

    private readonly BlockingCollection<QueuedItem> _queue = [];
    private readonly IJobSubscriptionHandler _handler;

    public QueueingScheduler(IJobSubscriptionHandler handler)
    {
        _handler = handler;
        Task.Factory.StartNew(HandleQueue, TaskCreationOptions.LongRunning);
    }

    private async Task HandleQueue()
    {
        while (true)
        {
            var item = _queue.Take();

            if (item.Subscription.CancellationToken.IsCancellationRequested)
            {
                continue;
            }
            else
            {
                await _handler.HandleAsync(item.Subscription);
                item.CompletionSource.SetResult();
            }
        }
    }

    public override async Task HandleAsync(JobSubscription jobSubscription)
    {
        QueuedItem item = new(jobSubscription);

        _queue.Add(item, jobSubscription.CancellationToken);

        if (!jobSubscription.CancellationToken.IsCancellationRequested)
        {
            await item.CompletionSource.Task;
        }
    }
}
