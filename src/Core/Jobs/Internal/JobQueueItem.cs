namespace Ocluse.LiquidSnow.Jobs.Internal
{
    internal record JobQueueItem(IJob Job, long Tick, CancellationToken CancellationToken);
}
