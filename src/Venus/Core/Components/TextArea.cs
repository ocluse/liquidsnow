using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that collects user input as a multi-line text.
/// </summary>
public class TextArea : InputControlBase<string>
{
    ///<inheritdoc/>
    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.OpenElement(10, "textarea");
        builder.AddAttribute(11, "onchange", OnChange);
        builder.AddAttribute(12, "placeholder", Placeholder ?? " ");
        builder.AddContent(13, Value);
        builder.CloseElement();
    }

    ///<inheritdoc/>
    protected override string GetValue(object? value)
    {
        return value?.ToString() ?? string.Empty;
    }
}
