namespace Ocluse.LiquidSnow.Venus.Models;

/// <summary>
/// The result of a dialog.
/// </summary>
public class DialogResult
{
    /// <summary>
    /// Indicates whether the dialog terminated it's operation successfully.
    /// </summary>
    public bool? Success { get; init; }

    /// <summary>
    /// Provides the data returned by the dialog.
    /// </summary>
    public object? Data { get; init; }
}
