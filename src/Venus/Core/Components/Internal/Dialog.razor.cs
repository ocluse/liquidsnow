namespace Ocluse.LiquidSnow.Venus.Components.Internal;

/// <summary>
/// A component that displays a dialog.
/// </summary>
public partial class Dialog
{
    private ElementReference _dialog;
    private string? _title;

    /// <summary>
    /// The properties of the dialog.
    /// </summary>
    [Parameter]
    public required DialogProperties Properties { get; set; }

    /// <summary>
    /// Gets or sets the type of component rendered as the content of the dialog.
    /// </summary>
    [Parameter]
    public required Type ComponentType { get; set; }

    /// <summary>
    /// Gets or sets the parameters to pass to the content component.
    /// </summary>
    [Parameter]
    public required Dictionary<string, object>? Parameters { get; set; }

    ///<inheritdoc/>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSInterop.ShowDialogAsync(_dialog);
        }
    }

    private async Task CloseDialogAsync()
    {
        await JSInterop.CloseDialogAsync(_dialog);
    }

    private void OnClose()
    {

    }

    public void UpdateTitle(string title)
    {

    }
}