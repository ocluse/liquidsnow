using Microsoft.JSInterop;

namespace Ocluse.LiquidSnow.Venus.Services;

internal sealed class VenusJSInterop(IJSRuntime jsRuntime) : IAsyncDisposable, IVenusJSInterop
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

    public async ValueTask InitializeDropdownWatcher()
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("initializeDropdownWatcher");
    }

    public async ValueTask WatchDropdownAsync(DotNetObjectReference<IDropdown> dropdown, string dropdownId)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("watchDropdown", dropdown, dropdownId);
    }

    public async ValueTask UnwatchDropdownAsync(DotNetObjectReference<IDropdown> dropdown)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("unwatchDropdown", dropdown);
    }

    public async ValueTask ShowPopoverAsync(ElementReference element)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("showPopover", element);
    }
}
