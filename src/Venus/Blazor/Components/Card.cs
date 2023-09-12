using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class Card : ControlBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public RenderFragment? Header { get; set; }

        [Parameter]
        public RenderFragment? Footer { get; set; }

        protected override void BuildClass(List<string> classList)
        {
            base.BuildClass(classList);
            classList.Add("card");
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", GetClass());
            builder.AddAttribute(2, "style", GetStyle());

            if(Header != null)
            {
                builder.OpenElement(3, "div");
                builder.AddAttribute(4, "class", "card-header");
                builder.AddContent(5, Header);
                builder.CloseElement();
            }

            builder.OpenElement(6, "div");
            builder.AddAttribute(7, "class", "card-body");
            builder.AddContent(8, ChildContent);
            builder.CloseElement();

            if(Footer != null)
            {
                builder.OpenElement(9, "div");
                builder.AddAttribute(10, "class", "card-footer");
                builder.AddContent(11, Footer);
                builder.CloseElement();
            }
            builder.CloseElement();
        }
    }
}
