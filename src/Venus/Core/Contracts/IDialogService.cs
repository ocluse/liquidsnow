namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// A service that provides methods for showing dialogs and loading messages.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Binds the host component that is responsible for displaying dialogs.
    /// </summary>
    void BindHost(IDialogHost host);

    /// <summary>
    /// Unbinds the host component when the host leaves the render tree.
    /// </summary>
    void UnbindHost(IDialogHost host);

    /// <summary>
    /// Shows the dialog of the provided type and returns the result once the dialog is closed.
    /// </summary>
    Task<DialogResult> ShowDialogAsync(Type dialogType, string? dialogHeader, bool allowDismiss, bool showClose, Dictionary<string, object?>? parameters);
    
    /// <summary>
    /// Shows the loading indicator with the provided message.
    /// </summary>
    void ShowLoading(string? message);
    
    /// <summary>
    /// Hides the loading indicator.
    /// </summary>
    void HideLoading();
}
