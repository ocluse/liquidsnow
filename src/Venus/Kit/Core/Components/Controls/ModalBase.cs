using Microsoft.AspNetCore.Components.Web;
using Ocluse.LiquidSnow.Venus.Kit.Services;

namespace Ocluse.LiquidSnow.Venus.Kit.Components.Controls;

public abstract class ModalBase : ComponentBase, IModal, ISystemBackReceiver, IDisposable
{
    [Inject]
    IVenusJSInterop JSInterop { get; set; } = default!;

    [Inject]
    SystemBackInterceptor SystemBackInterceptor { get; set; } = default!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public Func<Task<bool>>? OnDismissingFunc { get; set; }

    private bool _isVisible = false;
    private bool _isClosing = false;
    private ElementReference _dialogElement;
    private bool disposedValue;

    protected virtual string ContentAreaClass => "modal-content-area";

    public async Task ShowAsync()
    {
        if (_isVisible) return;

        _isVisible = true;
        _isClosing = false;

        SystemBackInterceptor.Bind(this);

        await InvokeAsync(StateHasChanged);

        await JSInterop.ShowDialogAsync(_dialogElement);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "dialog");
        {
            builder.AddAttribute(2, "class", GetClass());
            builder.AddAttribute(3, "onclick", EventCallback.Factory.Create(this, HideAsync));
            builder.AddAttribute(4, "onclose", EventCallback.Factory.Create(this, HandleDialogClose));
            builder.AddElementReferenceCapture(5, element => _dialogElement = element);
            if (_isVisible)
            {
                builder.OpenElement(5, "div");
                {
                    builder.AddAttribute(6, "class", ContentAreaClass);
                    builder.AddEventStopPropagationAttribute(7, "onclick", true);
                    builder.AddContent(8, ChildContent);
                }
                builder.CloseElement();
            }
        }
        builder.CloseElement();
    }
    public async Task HideAsync()
    {
        // Prevent hiding if already closing or not visible
        if (_isClosing || !_isVisible) return;

        if (OnDismissingFunc != null)
        {
            bool cancel = await OnDismissingFunc();
            if (cancel)
            {
                return;
            }
        }

        _isClosing = true; // Start the closing process

        await InvokeAsync(StateHasChanged);

        // Wait for the CSS animation to complete
        await Task.Delay(300);

        await JSInterop.CloseDialogAsync(_dialogElement);

        SystemBackInterceptor.Unbind(this);

        _isVisible = false;
        _isClosing = false;

        await InvokeAsync(StateHasChanged);
    }

    protected string GetClass()
    {
        ClassBuilder builder = new();

        BuildClass(builder);

        return builder
            .AddIf(_isVisible, "visible")
            .AddIf(_isClosing, "closing")
            .Build();
    }

    protected virtual void BuildClass(ClassBuilder builder)
    {
    }

    private async Task HandleDialogClose()
    {
        if (_isVisible && !_isClosing)
        {
            _isVisible = false;
            _isClosing = false;

            await InvokeAsync(StateHasChanged);
        }
    }

    public bool HandleBackPressed()
    {
        _ = HideAsync();
        return true;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                SystemBackInterceptor.Unbind(this);
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

