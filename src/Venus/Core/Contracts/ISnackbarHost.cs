namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// A contract for components that can show snackbar messages.
/// </summary>
public interface ISnackbarHost
{
    /// <summary>
    /// Shows the snackbar item represented by the supplied <see cref="SnackbarItemDescriptor"/>.
    /// </summary>
    Task ShowSnackbarAsync(SnackbarItemDescriptor descriptor, CancellationToken cancellationToken);
}
