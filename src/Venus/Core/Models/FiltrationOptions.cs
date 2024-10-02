using Ocluse.LiquidSnow.Venus.Components;

namespace Ocluse.LiquidSnow.Venus.Models;

/// <summary>
/// The filter and sort configuration for an <see cref="CollectionView{T}"/>
/// </summary>
public record FiltrationOptions
{
    /// <summary>
    /// The various options that the data can be filtered by.
    /// </summary>
    public required List<FilterOption> Filters { get; init; }

    /// <summary>
    /// The various options that the data can be sorted by.
    /// </summary>
    public required List<SortOption> Sorters { get; init; }

    /// <summary>
    /// The default sorting option applied when one is not provided.
    /// </summary>
    public required SortOption DefaultSort { get; init; }

    /// <summary>
    /// The default filtering option applied when one is not provided.
    /// </summary>
    public required FilterOption DefaultFilter { get; init; }
}
