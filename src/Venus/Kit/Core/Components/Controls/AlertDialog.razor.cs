namespace Ocluse.LiquidSnow.Venus.Kit.Components.Controls;

public partial class AlertDialog : IModal
{
    private Dialog? _innerDialog;

    [Parameter]
    public RenderFragment? Title { get; set; }

    [Parameter]
    public RenderFragment? Text { get; set; }

    [Parameter]
    public RenderFragment? Icon { get; set; }

    [Parameter]
    public Func<Task<bool>>? OnDismissingFunc { get; set; }

    [Parameter]
    public RenderFragment? DismissButton { get; set; }

    [Parameter]
    public RenderFragment? ConfirmButton { get; set; }

    [Parameter]
    public string? Class { get; set; }

    private string GetClass() => $"alert-dialog {Class}".Trim();

    public async Task HideAsync()
    {
        if (_innerDialog != null)
        {
            await ((IModal)_innerDialog).HideAsync();
        }
    }

    public async Task ShowAsync()
    {
        if (_innerDialog != null)
        {
            await ((IModal)_innerDialog).ShowAsync();
        }
    }