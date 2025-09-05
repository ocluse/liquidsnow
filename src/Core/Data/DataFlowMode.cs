namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Determines how the data flow will be received by collectors.
/// </summary>
public enum DataFlowMode
{
    /// <summary>
    /// The emitted data is handled normally and immediately.
    /// </summary>
    Normal,
    /// <summary>
    /// A debounce flow will wait for a specified delay before emitting the value.
    /// </summary>
    Debounce,
    /// <summary>
    /// The flow is throttled, meaning that once the value is emitted, it will not emit again until the specified delay has passed.
    /// Subsequent values will be ignored until the delay is over.
    /// </summary>
    Throttle,
    /// <summary>
    /// The flow is throttled, but it will emit the last value received after the delay has passed, even if new values were received during the delay.
    /// </summary>
    ThrottleTrailing
}
