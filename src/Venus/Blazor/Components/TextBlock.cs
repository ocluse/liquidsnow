using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class TextBlock : ControlBase
    {
        [Parameter]
        public int TextStyle { get; set; } = Values.TextStyle.Body;

        [Parameter]
        public int Hierarchy { get; set; } = TextHierarchy.Span;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, VenusResolver.ResolveTextHierarchy(Hierarchy));

            Dictionary<string, object> attributes = new()
            {
                { "class", GetClass() },
                {"style", GetStyle() },
            };

            builder.AddMultipleAttributes(1, attributes);

            if (ChildContent != null)
            {
                builder.AddContent(2, ChildContent);
            }

            builder.CloseElement();
        }

        protected override void BuildClass(List<string> classList)
        {
            base.BuildClass(classList);
            classList.Add(VenusResolver.ResolveTextStyle(TextStyle));
        }
    }
}
