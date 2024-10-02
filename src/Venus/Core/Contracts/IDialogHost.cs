namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// A contract for components that host dialogs.
/// </summary>
public interface IDialogHost
{
    /// <summary>
    /// Shows the dialog of the provided type and returns the result once the dialog is closed.
    /// </summary>
    Task<DialogResult> ShowDialog(Type dialogType, string? dialogHeader, bool allowDismiss, bool showClose, Dictionary<string, object?>? parameters);
    
    /// <summary>
    /// Closes the dialog. This method is typically called by the hosted dialog component.
    /// </summary>
    void CloseDialog(bool? isSuccess = null, object? data = null);

    /// <summary>
    /// Shows the loading indicator with the provided message.
    /// </summary>
    void ShowLoading(string? loadingMessage);

    /// <summary>
    /// Hides the loading indicator
    /// </summary>
    void HideLoading();
}
