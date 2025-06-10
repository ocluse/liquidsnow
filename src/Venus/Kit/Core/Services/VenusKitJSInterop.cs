using Microsoft.JSInterop;
using Ocluse.LiquidSnow.Venus.Kit.Utils;

namespace Ocluse.LiquidSnow.Venus.Kit.Services;
public sealed class VenusKitJSInterop(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", ContentHelper.GetContentPath("app.js")).AsTask());

    public async Task SetKeyboardInsetAsync(int newHeight)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("updateKeyboardInset", newHeight);
    }

    public async Task NavigateBackAsync()
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("navigateBack");
    }

    public async Task<bool> IsNearBottomAsync(ElementReference elementReference, int threshold = 100)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<bool>("isNearBottom", elementReference, threshold);
    }

    public async Task ScrollToBottomAsync(ElementReference elementReference)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("scrollToBottom", elementReference);
    }

    public async Task ScrollToTopAsync(ElementReference elementReference)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("scrollToTop", elementReference);
    }

    public async Task ScrollToPositionAsync(ElementReference elementReference, double position, bool isVertical)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("scrollToPosition", elementReference, position, isVertical);
    }

    public async Task<IJSObjectReference> CreateObjectAsync(string className, params object[] args)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<IJSObjectReference>($"create{className}", args);
    }

    public async Task<ElementScrollValues> GetScrollValuesAsync(ElementReference elementReference)
    {
        var module = await _moduleTask.Value;
        return await module.InvokeAsync<ElementScrollValues>("getScrollValues", elementReference);
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
