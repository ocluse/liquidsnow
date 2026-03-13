namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Extension methods that compose <see cref="IDataFlow{T}"/> operators, returning new flows that can be further chained or subscribed to.
/// </summary>
public static class DataFlowExtensions
{
    /// <summary>
    /// Returns a new flow that only emits values from the source flow that satisfy the predicate.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the flow.</typeparam>
    /// <param name="flow">The source flow.</param>
    /// <param name="predicate">A function that returns <see langword="true"/> for values that should be passed through.</param>
    /// <returns>An <see cref="IDataFlow{T}"/> that only emits values satisfying the predicate.</returns>
    public static IDataFlow<T> Filter<T>(this IDataFlow<T> flow, Func<T, bool> predicate)
    {
        return new FilteredDataFlow<T>(flow, predicate);
    }

    /// <summary>
    /// Returns a new flow that transforms each value emitted by the source flow using the provided selector.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the source flow.</typeparam>
    /// <typeparam name="TResult">The type of value emitted by the returned flow.</typeparam>
    /// <param name="flow">The source flow.</param>
    /// <param name="selector">A function that maps each emitted value to <typeparamref name="TResult"/>.</param>
    /// <returns>An <see cref="IDataFlow{TResult}"/> that emits transformed values.</returns>
    public static IDataFlow<TResult> Transform<T, TResult>(this IDataFlow<T> flow, Func<T, TResult> selector)
    {
        return new TransformedDataFlow<T, TResult>(flow, selector);
    }

    /// <summary>
    /// Returns a new flow that debounces values from the source flow.
    /// Each subscriber receives a value only after the source has been silent for <paramref name="delayMillis"/> milliseconds.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the flow.</typeparam>
    /// <param name="flow">The source flow.</param>
    /// <param name="delayMillis">The silence window in milliseconds. The timer resets each time a new value arrives.</param>
    /// <param name="maxWaitMillis">
    /// If greater than zero, guarantees that a value is dispatched at least once within this many milliseconds,
    /// even if the source keeps emitting within the silence window.
    /// </param>
    /// <returns>An <see cref="IDataFlow{T}"/> that debounces emitted values.</returns>
    public static IDataFlow<T> Debounce<T>(this IDataFlow<T> flow, int delayMillis, int maxWaitMillis = 0)
    {
        if (delayMillis < 0) throw new ArgumentOutOfRangeException(nameof(delayMillis), "Delay must be non-negative.");
        if (maxWaitMillis < 0) throw new ArgumentOutOfRangeException(nameof(maxWaitMillis), "Max wait time must be non-negative.");
        return new DebouncedDataFlow<T>(flow, delayMillis, maxWaitMillis);
    }

    /// <summary>
    /// Returns a new flow that throttles values from the source flow.
    /// The first value in each throttle window is dispatched immediately; subsequent values within the window are dropped.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the flow.</typeparam>
    /// <param name="flow">The source flow.</param>
    /// <param name="delayMillis">The length of the throttle window in milliseconds.</param>
    /// <param name="trailing">
    /// If <see langword="true"/>, the last value received during the throttle window is dispatched when the window closes.
    /// </param>
    /// <returns>An <see cref="IDataFlow{T}"/> that throttles emitted values.</returns>
    public static IDataFlow<T> Throttle<T>(this IDataFlow<T> flow, int delayMillis, bool trailing = false)
    {
        if (delayMillis < 0) throw new ArgumentOutOfRangeException(nameof(delayMillis), "Delay must be non-negative.");
        return new ThrottledDataFlow<T>(flow, delayMillis, trailing);
    }

    /// <summary>
    /// Returns a new flow that delivers every value emitted by the source flow, but limits how frequently
    /// they are dispatched to subscribers. Values that arrive faster than the interval are queued and
    /// dispatched one by one at the specified rate. No values are dropped.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the flow.</typeparam>
    /// <param name="flow">The source flow.</param>
    /// <param name="intervalMillis">The minimum interval in milliseconds between consecutive dispatches.</param>
    /// <returns>An <see cref="IDataFlow{T}"/> that rate-limits emitted values.</returns>
    public static IDataFlow<T> RateLimit<T>(this IDataFlow<T> flow, int intervalMillis)
    {
        if (intervalMillis < 0) throw new ArgumentOutOfRangeException(nameof(intervalMillis), "Interval must be non-negative.");
        return new RateLimitedDataFlow<T>(flow, intervalMillis);
    }
}
