namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// A service for showing snackbar messages.
/// </summary>
public interface ISnackbarService
{
    /// <summary>
    /// Sets the host component that will show the snackbar messages.
    /// </summary>
    void SetHost(ISnackbarHost host);

    /// <summary>
    /// Shows an error message notification on the snackbar stack.
    /// </summary>

    void AddError(string message, SnackbarDuration duration = SnackbarDuration.Medium);

    /// <summary>
    /// Shows a success message notification on the snackbar stack.
    /// </summary>
    void AddSuccess(string message, SnackbarDuration duration = SnackbarDuration.Medium);

    /// <summary>
    /// Shows an information message notification on the snackbar stack.
    /// </summary>
    void AddInformation(string message, SnackbarDuration duration = SnackbarDuration.Medium);

    /// <summary>
    /// Shows a warning message notification on the snackbar stack.
    /// </summary>
    void AddWarning(string message, SnackbarDuration duration = SnackbarDuration.Medium);
}
