namespace Ocluse.LiquidSnow.Events;

/// <summary>
/// Specifies the how an event bus invokes event handlers
/// </summary>
public enum PublishStrategy
{
    /// <summary>
    /// Invokes all handlers at once and waits until they all finish execution.
    /// </summary>
    Parallel,

    /// <summary>
    /// All handlers are invoked in a sequential format, stopping their execution at the first error.
    /// </summary>
    Sequential,

    /// <summary>
    /// Invokes all handlers and returns immediately, without waiting for any to finish their execution.
    /// </summary>
    FireAndForget,

    /// <summary>
    /// Invokes all handlers, returning after any handler finishes it's execution
    /// </summary>
    FireAndForgetAfterFirst
}