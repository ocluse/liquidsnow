namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// A contract for components that host dialogs.
/// </summary>
public interface IDialogHost
{
    /// <summary>
    /// Shows the dialog represented by the supplied <see cref="DialogDescriptor"/> and returns the result
    /// </summary>
    Task<DialogResult> ShowDialogAsync(DialogDescriptor descriptor, CancellationToken cancellationToken);
}
