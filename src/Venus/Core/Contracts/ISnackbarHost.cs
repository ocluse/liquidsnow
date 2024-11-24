namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// A contract for components that can show snackbar messages.
/// </summary>
public interface ISnackbarHost
{
    /// <summary>
    /// Shows a snackbar message.
    /// </summary>
    ISnackbarItemHandle ShowMessage(SnackbarMessage message);
}
