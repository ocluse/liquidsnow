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
    /// dispatched one by one at the specified rate. No values are dropped unless <paramref name="maxQueueSize"/> is set.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the flow.</typeparam>
    /// <param name="flow">The source flow.</param>
    /// <param name="intervalMillis">The minimum interval in milliseconds between consecutive dispatches.</param>
    /// <param name="maxQueueSize">
    /// The maximum number of values that may be queued awaiting dispatch.
    /// When zero (the default), the queue is unbounded.
    /// </param>
    /// <param name="queueOverflowBehavior">
    /// Determines what happens when an incoming value arrives and the queue is already at <paramref name="maxQueueSize"/>.
    /// <see cref="BufferOverflowBehavior.DropOldest"/> removes the oldest queued item to make room;
    /// <see cref="BufferOverflowBehavior.DropNewest"/> discards the incoming value.
    /// Ignored when <paramref name="maxQueueSize"/> is zero.
    /// </param>
    /// <returns>An <see cref="IDataFlow{T}"/> that rate-limits emitted values.</returns>
    public static IDataFlow<T> RateLimit<T>(
        this IDataFlow<T> flow,
        int intervalMillis,
        int maxQueueSize = 0,
        BufferOverflowBehavior queueOverflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        if (intervalMillis < 0) throw new ArgumentOutOfRangeException(nameof(intervalMillis), "Interval must be non-negative.");
        if (maxQueueSize < 0) throw new ArgumentOutOfRangeException(nameof(maxQueueSize), "Max queue size must be non-negative.");
        return new RateLimitedDataFlow<T>(flow, intervalMillis, maxQueueSize, queueOverflowBehavior);
    }

    /// <summary>
    /// Returns a new flow that invokes a side-effect action on each value before forwarding it unchanged.
    /// Useful for logging or diagnostics mid-chain without breaking the chain type.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the flow.</typeparam>
    /// <param name="flow">The source flow.</param>
    /// <param name="sideEffect">The action to invoke on each value. Its return value is ignored.</param>
    /// <returns>An <see cref="IDataFlow{T}"/> that emits the same values as the source flow.</returns>
    public static IDataFlow<T> Do<T>(this IDataFlow<T> flow, Action<T> sideEffect)
    {
        return new DoDataFlow<T>(flow, sideEffect);
    }

    /// <summary>
    /// Returns a new flow that collects values from the source flow and emits them as a batch once
    /// the specified number of values has been accumulated.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the source flow.</typeparam>
    /// <param name="flow">The source flow.</param>
    /// <param name="maxSize">The number of values to accumulate before emitting a batch. Must be greater than zero.</param>
    /// <param name="flushBehavior">
    /// Determines what happens to a partial batch remaining in the buffer when the subscription is disposed.
    /// </param>
    /// <returns>An <see cref="IDataFlow{T}"/> that emits batches of up to <paramref name="maxSize"/> values.</returns>
    public static IDataFlow<IReadOnlyList<T>> BatchByCount<T>(
        this IDataFlow<T> flow,
        int maxSize,
        BatchFlushBehavior flushBehavior = BatchFlushBehavior.Discard)
    {
        if (maxSize <= 0) throw new ArgumentOutOfRangeException(nameof(maxSize), "Max size must be greater than zero.");
        return new BatchedDataFlow<T>(flow, maxSize, 0, flushBehavior);
    }

    /// <summary>
    /// Returns a new flow that collects values from the source flow and emits them as a batch on a fixed time interval.
    /// An empty batch is emitted if no values arrived during the window.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the source flow.</typeparam>
    /// <param name="flow">The source flow.</param>
    /// <param name="windowMillis">The length of the time window in milliseconds. Must be greater than zero.</param>
    /// <param name="flushBehavior">
    /// Determines what happens to a partial batch remaining in the buffer when the subscription is disposed.
    /// </param>
    /// <returns>An <see cref="IDataFlow{T}"/> that emits batches of values on each window tick.</returns>
    public static IDataFlow<IReadOnlyList<T>> BatchByWindow<T>(
        this IDataFlow<T> flow,
        int windowMillis,
        BatchFlushBehavior flushBehavior = BatchFlushBehavior.Discard)
    {
        if (windowMillis <= 0) throw new ArgumentOutOfRangeException(nameof(windowMillis), "Window duration must be greater than zero.");
        return new BatchedDataFlow<T>(flow, 0, windowMillis, flushBehavior);
    }

    /// <summary>
    /// Returns a new flow that suppresses consecutive duplicate values.
    /// A value is only forwarded if it differs from the previously emitted value.
    /// The first value is always forwarded.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the flow.</typeparam>
    /// <param name="flow">The source flow.</param>
    /// <param name="comparer">
    /// The equality comparer used to determine whether two consecutive values are equal.
    /// When <see langword="null"/>, <see cref="EqualityComparer{T}.Default"/> is used.
    /// </param>
    /// <returns>An <see cref="IDataFlow{T}"/> that only emits values that differ from the previous emission.</returns>
    public static IDataFlow<T> DistinctUntilChanged<T>(
        this IDataFlow<T> flow,
        IEqualityComparer<T>? comparer = null)
    {
        return new DistinctUntilChangedDataFlow<T>(flow, comparer ?? EqualityComparer<T>.Default);
    }

    /// <summary>
    /// Returns a new flow that samples the source flow on a fixed interval, emitting the most recently
    /// received value on each tick.
    /// </summary>
    /// <typeparam name="T">The type of value emitted by the flow.</typeparam>
    /// <param name="flow">The source flow.</param>
    /// <param name="intervalMillis">The sampling interval in milliseconds. Must be greater than zero.</param>
    /// <param name="emptyBehavior">
    /// Determines what happens when the timer ticks but no new value has arrived since the last emission.
    /// <see cref="SampleEmptyBehavior.Skip"/> emits nothing;
    /// <see cref="SampleEmptyBehavior.ReplayLast"/> re-emits the last known value.
    /// The very first tick is always suppressed if no value has ever arrived.
    /// </param>
    /// <returns>An <see cref="IDataFlow{T}"/> that emits sampled values at the specified interval.</returns>
    public static IDataFlow<T> Sample<T>(
        this IDataFlow<T> flow,
        int intervalMillis,
        SampleEmptyBehavior emptyBehavior = SampleEmptyBehavior.Skip)
    {
        if (intervalMillis <= 0) throw new ArgumentOutOfRangeException(nameof(intervalMillis), "Interval must be greater than zero.");
        return new SampledDataFlow<T>(flow, intervalMillis, emptyBehavior);
    }
}
