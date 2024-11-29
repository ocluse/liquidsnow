using Microsoft.JSInterop;

namespace Ocluse.LiquidSnow.Venus.Services;

internal sealed class VenusJSInterop(IJSRuntime jsRuntime) : IAsyncDisposable, IVenusJSInterop
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Ocluse.LiquidSnow.Venus/venus.js").AsTask());

    public async ValueTask CloseDialogAsync(ElementReference dialog)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("closeDialog", dialog);
    }

    public async ValueTask ShowDialogAsync(ElementReference dialog)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("showDialog", dialog);
    }

    public async ValueTask ShowPopoverAsync(ElementReference element)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("showPopover", element);
    }

    public async ValueTask HidePopoverAsync(ElementReference element)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("hidePopover", element);
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
