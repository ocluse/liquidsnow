namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// A handle to a snackbar message being shown, allowing it to be closed.
/// </summary>
public interface ISnackbarItemHandle
{
    /// <summary>
    /// The content of the snackbar message.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// The status of the snackbar message.
    /// </summary>
    int Status { get; }

    /// <summary>
    /// The duration for which the snackbar message will be shown.
    /// </summary>
    SnackbarDuration Duration { get; }

    /// <summary>
    /// A method to close the snackbar message manually.
    /// </summary>
    void Close();

}
