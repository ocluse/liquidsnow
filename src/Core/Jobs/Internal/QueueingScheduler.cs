using System.Collections.Concurrent;

namespace Ocluse.LiquidSnow.Jobs.Internal
{
    internal class QueueingScheduler : CoreScheduler
    {
        internal record QueuedItem(IJob Job, long Tick, CancellationToken CancellationToken)
        {
            public TaskCompletionSource CompletionSource { get; } = new();
        }

        private readonly BlockingCollection<QueuedItem> _queue = [];
        private readonly IJobExecutor _executor;

        public QueueingScheduler(IJobExecutor executor)
        {
            _executor = executor;
            Task.Factory.StartNew(HandleQueue, TaskCreationOptions.LongRunning);
        }

        private async Task HandleQueue()
        {
            while (true)
            {
                var item = _queue.Take();

                if (item.CancellationToken.IsCancellationRequested)
                {
                    continue;
                }
                else
                {
                    await _executor.Execute(item.Job, item.Tick, item.CancellationToken);
                    item.CompletionSource.SetResult();
                }
            }
        }

        public override async Task Execute(IJob job, long tick, CancellationToken cancellationToken)
        {
            QueuedItem item = new(job, tick, cancellationToken);
            _queue.Add(item, cancellationToken: default);
            if(!cancellationToken.IsCancellationRequested)
            await item.CompletionSource.Task;
        }
    }
}
