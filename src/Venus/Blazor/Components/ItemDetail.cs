using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class ItemDetail : ControlBase
    {
        [Parameter]
        public string? Item { get; set; }

        [Parameter]
        public RenderFragment? ItemContent { get; set; }

        [Parameter]
        public string? Detail { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (ItemContent != null)
            {
                builder.OpenElement(3, "dt");
                builder.AddMultipleAttributes(4, GetClassAndStyle());
                builder.AddContent(5, ItemContent);
                builder.CloseElement();
            }
            else if (Item != null)
            {
                builder.OpenElement(0, "dt");
                builder.AddMultipleAttributes(1, GetClassAndStyle());
                builder.AddContent(2, Item);
                builder.CloseElement();
            }

            if (ChildContent != null)
            {
                builder.OpenElement(6, "dd");
                builder.AddMultipleAttributes(7, GetClassAndStyle());
                builder.AddContent(8, ChildContent);
                builder.CloseElement();
            }
            else if (Detail != null)
            {
                builder.OpenElement(3, "dd");
                builder.AddMultipleAttributes(4, GetClassAndStyle());
                builder.AddContent(5, Detail);
                builder.CloseElement();
            }
        }
    }
}
