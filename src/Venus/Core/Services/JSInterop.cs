using Microsoft.JSInterop;

namespace Ocluse.LiquidSnow.Venus.Services;

internal class JSInterop(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Ocluse.LiquidSnow.Venus/venus.js").AsTask());

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }

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
}
