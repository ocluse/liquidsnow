using Microsoft.AspNetCore.Components;
using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{

    public class TextBox : InputControlBase<string>
    {
        [Parameter]
        public Keyboard InputType { get; set; }

        protected override string? GetValue(object? value)
        {
            return value?.ToString() ?? string.Empty;
        }

        protected override string GetInputType()
        {
            return InputType.ToString().PascalToKebabCase();
        }
    }
}
