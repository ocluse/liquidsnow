namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// A handle to a snackbar message being shown, allowing it to be closed.
/// </summary>
public interface ISnackbarItemHandle
{
    /// <summary>
    /// Closes the snackbar message manually.
    /// </summary>
    Task CloseAsync();

}
