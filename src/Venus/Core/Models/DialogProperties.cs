namespace Ocluse.LiquidSnow.Venus.Models;

/// <summary>
/// Encapsulates the properties of a dialog.
/// </summary>
public record DialogProperties
{
    /// <summary>
    /// Gets or sets the title of the dialog.
    /// </summary>
    public string? Title { get; init; }

    /// <summary>
    /// Gets or sets whether a close button should be shown.
    /// </summary>
    public bool ShowClose { get; init; }

}
