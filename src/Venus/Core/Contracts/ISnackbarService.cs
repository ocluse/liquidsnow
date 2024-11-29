namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// A service for rendering snackbar items.
/// </summary>
public interface ISnackbarService
{
    /// <summary>
    /// Binds the host component that will render the snackbar items.
    /// </summary>
    void BindHost(ISnackbarHost host);

    /// <summary>
    /// Unbinds the host component if it currently the one set to render snackbar items.
    /// </summary>
    void UnbindHost(ISnackbarHost host);

    /// <summary>
    /// Renders the specified snackbar item to the host and waits until it is closed.
    /// </summary>
    Task ShowSnackbarItemAsync(SnackbarItemDescriptor descriptor, CancellationToken cancellationToken = default);
}
