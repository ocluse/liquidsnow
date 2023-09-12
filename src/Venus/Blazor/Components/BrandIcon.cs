using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class BrandIcon : ControlBase
    {
        [Parameter]
        public string? Icon { get; set; }

        [Parameter]
        public int Size { get; set; } = DefaultSize.Size24;


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
                    { "viewBox", "0 0 48 48" },
                };

                builder.AddMultipleAttributes(1, _attributes);

                MarkupString content = new(Icon);

                builder.AddContent(3, content);
                builder.CloseElement();
            }
        }
    }
}
