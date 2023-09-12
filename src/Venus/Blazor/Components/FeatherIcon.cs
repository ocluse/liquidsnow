using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{

    public class FeatherIcon : ControlBase
    {
        public const int STROKE_WIDTH = 2;
        [Parameter]
        public string? Icon { get; set; }

        [Parameter]
        public int Size { get; set; } = DefaultSize.Size24;

        [Parameter]
        public int StrokeWidth { get; set; } = STROKE_WIDTH;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (!string.IsNullOrEmpty(Icon))
            {
                builder.OpenElement(0, "svg");

                Dictionary<string, object> _attributes = new()
                {
                    { "height", Size},
                    { "width", Size},
                    { "class", GetClass() },
                    { "style", GetStyle() },
                    { "xmlns", "http://www.w3.org/2000/svg" },
                    { "viewBox", "0 0 24 24" },
                    { "fill", "none" },
                    { "stroke", "currentColor" },
                    { "stroke-width",StrokeWidth },
                    { "stroke-linecap","round"},
                    { "stroke-linejoin", "round"}
                };

                builder.AddMultipleAttributes(1, _attributes);

                MarkupString content = new(Icon);

                builder.AddContent(3, content);
                builder.CloseElement();
            }
        }
    }
}
