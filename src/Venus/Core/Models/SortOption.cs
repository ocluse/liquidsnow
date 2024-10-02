namespace Ocluse.LiquidSnow.Venus.Models;

/// <summary>
/// An option for sorting a list of items.
/// </summary>
public record SortOption(Ordering Ordering, string Name, object Value) : FilterOption(Name, Value);
