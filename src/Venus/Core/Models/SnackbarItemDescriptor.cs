namespace Ocluse.LiquidSnow.Venus.Models;

/// <summary>
/// Defines properties used to construct a snackbar item.
/// </summary>
public record SnackbarItemDescriptor
{
    /// <summary>
    /// Gets or initializes the duration for which the snackbar item will be displayed.
    /// </summary>
    public required SnackbarDuration Duration { get; init; }

    /// <summary>
    /// Gets or initializes the component type to render on the snackbar.
    /// </summary>
    public required Type ContentType { get; init; }

    /// <summary>
    /// Gets or initializes the parameters applied on the content component.
    /// </summary>
    public IEnumerable<KeyValuePair<string, object?>>? Parameters { get; init; }
}