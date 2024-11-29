namespace Ocluse.LiquidSnow.Extensions;

/// <summary>
/// Extensions for the task Namespace
/// </summary>
public static class TaskExtensions
{
    ///<inheritdoc cref="TimeoutAfter(Task, TimeSpan)"/>
    public static Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, double milliseconds)
    {
        return task.TimeoutAfter(TimeSpan.FromMilliseconds(milliseconds));
    }

    ///<inheritdoc cref="TimeoutAfter(Task, TimeSpan)"/>
    public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
    {

        using CancellationTokenSource timeoutCancellationTokenSource = new();

        Task completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
        if (completedTask == task)
        {
            timeoutCancellationTokenSource.Cancel();
            return await task;  // Very important in order to propagate exceptions
        }
        else
        {
            throw new TimeoutException("The operation has timed out.");
        }
    }

    ///<inheritdoc cref="TimeoutAfter(Task, TimeSpan)"/>
    public static Task TimeoutAfter(this Task task, double milliseconds)
    {
        return task.TimeoutAfter(TimeSpan.FromMilliseconds(milliseconds));
    }

    /// <summary>
    /// Timeout a task after the provided duration.
    /// </summary>
    /// <exception cref="TimeoutException"></exception>
    public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
    {

        using CancellationTokenSource timeoutCancellationTokenSource = new();

        Task completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
        if (completedTask == task)
        {
            timeoutCancellationTokenSource.Cancel();
            await task;  // Very important in order to propagate exceptions
        }
        else
        {
            throw new TimeoutException("The operation has timed out.");
        }
    }

    /// <summary>
    /// A workaround for getting all of AggregateException.InnerExceptions with try/await/catch
    /// </summary>
    /// <remarks>
    /// Answer was originally posted on  <see href="https://stackoverflow.com/a/62607500/6701056">StackOverflow</see>
    /// </remarks>
    public static Task WithAggregatedExceptions(this Task task)
    {
        // using AggregateException.Flatten as a bonus
        return task.ContinueWith(
            continuationFunction: anteTask =>
                anteTask.IsFaulted &&
                anteTask.Exception is AggregateException ex &&
                (ex.InnerExceptions.Count > 1 || ex.InnerException is AggregateException) ?
                Task.FromException(ex.Flatten()) : anteTask,
            cancellationToken: CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            scheduler: TaskScheduler.Default).Unwrap();
    }
}
