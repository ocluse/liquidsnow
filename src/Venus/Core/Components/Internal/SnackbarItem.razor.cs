namespace Ocluse.LiquidSnow.Venus.Components.Internal;

/// <summary>
/// A component that displays a single snackbar message
/// </summary>
public partial class SnackbarItem
{
    /// <summary>
    /// Gets or sets the handle that is used to control the snackbar item.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public required ISnackbarItemHandle Handle { get; set; }

    /// <summary>
    /// Gets or sets the callback that is invoked when the snackbar item is closed.
    /// </summary>
    [Parameter]
    public EventCallback<SnackbarMessage> OnClose { get; set; }

    private void OnClickClose()
    {
        Handle.Close();
    }
}