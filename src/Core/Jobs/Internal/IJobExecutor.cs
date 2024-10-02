namespace Ocluse.LiquidSnow.Jobs.Internal;

internal interface IJobExecutor
{
    Task Execute(IJob job, long tick, CancellationToken cancellationToken);
}
