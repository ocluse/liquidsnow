using Microsoft.JSInterop;
using Ocluse.LiquidSnow.Validations;
using Ocluse.LiquidSnow.Venus.Kit.Services;

namespace Ocluse.LiquidSnow.Venus.Kit.Components.Controls;
public sealed partial class InputBox : IAsyncDisposable
{
    private string? _value;

    [Parameter]
    public string? Placeholder { get; set; }

    [Parameter]
    public int MaxLines { get; set; } = 4;

    [Inject]
    private VenusKitJSInterop JSInterop { get; set; } = null!;

    private ElementReference _textAreaElement;
    private IJSObjectReference? _jsInstance;

    protected override void BuildInputClass(ClassBuilder builder)
    {
        builder.Add("input-box");
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _jsInstance = await JSInterop.CreateObjectAsync("InputBox", _textAreaElement, MaxLines);
            await _jsInstance.InvokeVoidAsync("adjustHeight");
        }
        else if (_jsInstance != null)
        {
            await _jsInstance.InvokeVoidAsync("adjustHeight");
        }
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        bool maxLinesChanged = parameters.TryGetValue(nameof(MaxLines), out int maxLines) && maxLines != MaxLines;

        await base.SetParametersAsync(parameters);

        if (maxLinesChanged)
        {
            if (_jsInstance is not null)
            {
                await _jsInstance.InvokeVoidAsync("setMaxLines", maxLines);
            }
        }
    }

    protected override void OnParametersSet()
    {
        if (_value != Value)
        {
            _value = Value;
        }
        base.OnParametersSet();
    }

    private async Task HandleOnChange(ChangeEventArgs args)
    {
        _value = args.Value?.ToString();

        await NotifyValueChange(_value);
    }

    public async ValueTask DisposeAsync()
    {
        if (_jsInstance != null)
        {
            await _jsInstance.InvokeVoidAsync("dispose");
            await _jsInstance.DisposeAsync();
            _jsInstance = null;
        }
    }

    public override async Task<bool> InvokeValidate()
    {
        bool result;
        if (Validate != null)
        {
            ValidationResult validationResult = await Validate(Value);
            await ValidationChanged.InvokeAsync(validationResult);
            result = validationResult.IsValid;
        }
        else
        {
            result = true;
        }
        return result;
    }
}