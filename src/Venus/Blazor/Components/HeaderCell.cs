using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class HeaderCell : ControlBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "th");

            builder.AddMultipleAttributes(1, GetClassAndStyle());
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }
    }
}
