namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Determines how the buffer of values behaves when it reaches its maximum size.
/// </summary>
public enum BufferOverflowBehavior
{
    /// <summary>
    /// The oldest value in the buffer will be dropped to make room for the new value.
    /// </summary>
    DropOldest,
    /// <summary>
    /// The new value will be ignored if the buffer is full, effectively dropping it.
    /// </summary>
    DropNewest,
}
