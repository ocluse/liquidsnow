using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that allows a user to input multiple lines of text.
/// </summary>
public class TextArea : TextBoxBase<string>
{
    ///<inheritdoc/>
    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "textarea");
        {
            builder.AddAttribute(2, "onchange", EventCallback.Factory.CreateBinder(this, HandleValueUpdated, Value ?? string.Empty));
            builder.AddAttribute(3, "placeholder", Placeholder ?? " ");
            builder.AddContent(4, Value);
        }
        builder.CloseElement();
    }

    ///<inheritdoc/>
    protected override string GetValue(object? value)
    {
        return value?.ToString() ?? string.Empty;
    }

    private async Task HandleValueUpdated(string? value)
    {
        ChangeEventArgs e = new()
        {
            Value = value
        };

        await HandleInputChange(e);
    }
}
