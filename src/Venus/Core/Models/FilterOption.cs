namespace Ocluse.LiquidSnow.Venus.Models;

/// <summary>
/// An option for filtering a list of items.
/// </summary>
/// <param name="Name">The name that is displayed in the UI</param>
/// <param name="Value">The value that is sent with the <see cref="PaginationState.Sort"/></param>
public record FilterOption(string Name, object Value);
