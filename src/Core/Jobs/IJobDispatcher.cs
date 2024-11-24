namespace Ocluse.LiquidSnow.Jobs;

/// <summary>
/// Defines methods responsible for routing jobs to their designated handlers.
/// </summary>
public interface IJobDispatcher
{
    /// <summary>
    /// Dispatches the job to its designated handler.
    /// </summary>
    Task DispatchAsync(object job, Type jobType, long tick, bool throwExceptions, CancellationToken cancellationToken = default);

    ///<inheritdoc cref="DispatchAsync(object, Type, long, bool,CancellationToken)"/>
    Task DispatchAsync<T>(T job, long tick, bool throwExceptions, CancellationToken cancellationToken = default);
}
