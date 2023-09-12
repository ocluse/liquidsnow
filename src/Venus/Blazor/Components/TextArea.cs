using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class TextArea : InputControlBase<string>
    {
        protected override void BuildInput(RenderTreeBuilder builder)
        {
            builder.OpenElement(10, "textarea");
            builder.AddAttribute(11, "onchange", OnChange);
            builder.AddAttribute(12, "placeholder", Placeholder ?? " ");
            builder.AddContent(13, Value);
            builder.CloseElement();
        }

        protected override string GetValue(object? value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
