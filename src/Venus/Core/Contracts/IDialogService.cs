namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// Defines a service that is used to show dialogs.
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
    Task<DialogResult> ShowDialogAsync(DialogDescriptor descriptor, CancellationToken cancellationToken = default);
    
}
