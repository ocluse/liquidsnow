namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Determines how a batched data flow handles a partial (unflushed) buffer when the subscription is disposed.
/// </summary>
public enum BatchFlushBehavior
{
    /// <summary>
    /// Any values remaining in the buffer at disposal time are silently discarded.
    /// </summary>
    Discard,

    /// <summary>
    /// Any values remaining in the buffer at disposal time are emitted as a final batch before the subscription is torn down.
    /// </summary>
    Flush,
}
