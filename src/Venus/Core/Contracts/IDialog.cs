namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// Defines a dialog.
/// </summary>
public interface IDialog
{
    /// <summary>
    /// Updates the CSS classes of dialog to the provided classes.
    /// </summary>
    Task UpdateDialogClassesAsync(string classes);
    
    /// <summary>
    /// Updates the CSS classes of the dialog to those defined in the builder.
    /// </summary>
    Task UpdateDialogClassesAsync(ClassBuilder builder);

    /// <summary>
    /// Closes the dialog with the provided result.
    /// </summary>
    Task CloseAsync(bool? success, object? data);
}

/// <summary>
/// Defines a snackbar item.
/// </summary>
public interface ISnackbarItem
{
    /// <summary>
    /// Closes the snackbar manually.
    /// </summary>
    Task CloseAsync();
}