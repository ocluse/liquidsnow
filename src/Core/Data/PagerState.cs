namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Represents the state of a <see cref="Pager{TKey, TItem}"/>'s load operations.
/// </summary>
public record PagerState
{
    /// <summary>
    /// The state of the refresh operation.
    /// </summary>
    public required LoadState Refresh { get; init; }

    /// <summary>
    /// The state of the append operation.
    /// </summary>
    public required LoadState Append { get; init; }

    /// <summary>
    /// The state of the prepend operation.
    /// </summary>
    public required LoadState Prepend { get; init; }
}
