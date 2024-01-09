using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class ItemDetailContainer : ControlBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "dl");
            builder.AddMultipleAttributes(1, GetClassAndStyle());
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }
        protected override void BuildClass(ClassBuilder builder)
        {
            builder.Add("item-detail-container");
        }
    }
}
