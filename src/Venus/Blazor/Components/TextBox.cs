using Microsoft.AspNetCore.Components;
using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{

    public class TextBox : InputControlBase<string>
    {
        [Parameter]
        public Keyboard Keyboard { get; set; }

        protected override string InputType => Keyboard.ToString().PascalToKebabCase();

        protected override string? GetValue(object? value)
        {
            return value?.ToString() ?? string.Empty;
        }
    }
}
