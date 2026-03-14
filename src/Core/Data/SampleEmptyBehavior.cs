namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Determines how a sampled data flow behaves when the timer ticks but no new value has arrived since the last emission.
/// </summary>
public enum SampleEmptyBehavior
{
    /// <summary>
    /// Nothing is emitted on a tick if no new value has arrived since the last emission.
    /// </summary>
    Skip,

    /// <summary>
    /// The last known value is re-emitted on every tick.
    /// The very first tick is still suppressed if no value has ever arrived.
    /// </summary>
    ReplayLast,
}
