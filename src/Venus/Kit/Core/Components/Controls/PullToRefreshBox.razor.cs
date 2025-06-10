using Microsoft.JSInterop;
using Ocluse.LiquidSnow.Venus.Kit.Services;

namespace Ocluse.LiquidSnow.Venus.Kit.Components.Controls;
public partial class PullToRefreshBox
{
    [Parameter]
    public bool IsRefreshing { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment? RefreshIndicatorContent { get; set; }

    [Parameter]
    public RenderFragment? PeekContent { get; set; }

    [Parameter]
    public EventCallback RefreshRequested { get; set; }

    [Inject]
    public VenusKitJSInterop JSInterop { get; set; } = null!;

    private DotNetObjectReference<PullToRefreshBox>? _dotNetObjRef;
    private IJSObjectReference? _jsObjectRef;
    private ScrollBox? _scrollBox;
    bool _created;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_created)
        {
            if (_scrollBox != null && _jsObjectRef != null && _scrollBox.ScrollElement.Context != null)
            {
                await _jsObjectRef.InvokeVoidAsync("setElement", _scrollBox.ScrollElement);
            }
        }
        else
        {
            if (_scrollBox != null && _scrollBox.ScrollElement.Context != null)
            {
                _dotNetObjRef = DotNetObjectReference.Create(this);
                _jsObjectRef = await JSInterop.CreateObjectAsync("PullToRefreshBox", _scrollBox.ScrollElement, _dotNetObjRef);
                _created = true;
            }
        }
    }

    protected override void BuildClass(ClassBuilder builder)
    {
        base.BuildClass(builder);
        builder.Add("pull-to-refresh-box");
    }

    [JSInvokable]
    public async Task HandleRefresh()
    {
        await InvokeAsync(RefreshRequested.InvokeAsync);
    }

    public async ValueTask DisposeAsync()
    {
        if (_jsObjectRef is not null)
        {
            await _jsObjectRef.InvokeVoidAsync("dispose");
            await _jsObjectRef.DisposeAsync();
        }

        _dotNetObjRef?.Dispose();
    }
}