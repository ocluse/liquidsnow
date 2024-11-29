using System.Reactive.Linq;

namespace Ocluse.LiquidSnow.Jobs.Internal;

internal sealed class JobSubscription(IJob job, IJobSubscriptionHandler handler, Type jobType) : IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    
    private IDisposable? _handle;

    public IJob Job { get; } = job;

    public Type JobType { get; } = jobType;

    public long CurrentTick { get; private set; }

    public CancellationToken CancellationToken => _cts.Token;

    public event EventHandler<JobSubscription>? Disposed;

    public void Activate()
    {
        if (Job is IRoutineJob routineJob && Job is not ITaskSeriesJob)
        {
            _handle = Subscribe(Job.Start, routineJob.Interval);
        }
        else
        {
            _handle = Subscribe(Job.Start, null);
        }
    }

    private IDisposable Subscribe(DateTimeOffset startTime, TimeSpan? interval)
    {
        IObservable<long> observable;

        if (interval is not null)
        {
            observable = Observable.Timer(startTime, interval.Value);
        }
        else
        {
            observable = Observable.Timer(startTime);
        }

        return observable
             .Subscribe(async tick =>
             {
                 //Task series sets its own tick
                 if (Job is not ITaskSeriesJob)
                 {
                     CurrentTick = tick;
                 }

                 await DispatchAsync();
             });
    }

    private async Task DispatchAsync()
    {
        await handler.HandleAsync(this);
        //await dispatcher.DispatchAsync(Job, JobType, CurrentTick, false, _cts.Token);

        if (Job is ITaskSeriesJob or not IRoutineJob)
        {
            if (Job is ITaskSeriesJob taskSeriesJob)
            {
                CurrentTick++;
                _handle = Subscribe(DateTimeOffset.Now + taskSeriesJob.Interval, null);
            }
            else
            {
                Dispose();
            }
        }
    }

    public void Dispose()
    {
        _handle?.Dispose();
        _cts.Cancel();

        Disposed?.Invoke(this, this);
    }
}
