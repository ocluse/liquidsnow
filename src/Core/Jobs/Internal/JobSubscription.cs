using System.Reactive.Linq;

namespace Ocluse.LiquidSnow.Jobs.Internal;

internal class JobSubscription(IJob job, IJobExecutor executor) : IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    private long _currentTick;
    protected IDisposable? _handle;

    public IJob Job { get; } = job;

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
                     _currentTick = tick;
                 }

                 await Execute();
             });
    }

    private async Task Execute()
    {
        await executor.Execute(Job, _currentTick, _cts.Token);

        if (Job is ITaskSeriesJob or not IRoutineJob)
        {
            if (Job is ITaskSeriesJob taskSeriesJob)
            {
                _currentTick++;
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
