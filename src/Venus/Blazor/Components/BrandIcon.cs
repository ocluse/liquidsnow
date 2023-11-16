using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class BrandIcon : ControlBase
    {
        [Parameter]
        public string? Icon { get; set; }

        [Parameter]
        public int? Size { get; set; }


        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (!string.IsNullOrEmpty(Icon))
            {
                builder.OpenElement(0, "svg");

                Dictionary<string, object> attributes = new()
                {
                    { "height", Size ?? Resolver.DefaultIconSize},
                    { "width", Size ?? Resolver.DefaultIconSize},
                    { "xmlns", "http://www.w3.org/2000/svg" },
                    { "viewBox", "0 0 48 48" },
                };

                builder.AddMultipleAttributes(1, attributes);
                builder.AddMultipleAttributes(2, GetClassAndStyle());
                MarkupString content = new(Icon);

                builder.AddContent(3, content);
                builder.CloseElement();
            }
        }
    }
}
