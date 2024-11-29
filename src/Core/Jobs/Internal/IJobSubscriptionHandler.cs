namespace Ocluse.LiquidSnow.Jobs.Internal;

internal interface IJobSubscriptionHandler
{
    Task HandleAsync(JobSubscription jobSubscription);
}
