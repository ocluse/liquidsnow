namespace Ocluse.LiquidSnow.Venus.Models;

/// <summary>
/// A message to be shown in a snackbar.
/// </summary>
public record SnackbarMessage(string Content, int Status, SnackbarDuration Duration);