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

        [Parameter]
        public string? ContentClass { get; set; }

        [Parameter]
        public string? HeaderClass { get; set; }

        [Parameter]
        public string? FooterClass { get; set; }


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
                string headerClass = HeaderClass ?? "card-header";
                
                builder.OpenElement(3, "div");
                builder.AddAttribute(4, "class", headerClass);
                builder.AddContent(5, Header);
                builder.CloseElement();
            }

            string contentClass = ContentClass ?? "card-body";
            builder.OpenElement(6, "div");
            builder.AddAttribute(7, "class", contentClass);
            builder.AddContent(8, ChildContent);
            builder.CloseElement();

            if(Footer != null)
            {
                string footerClass = FooterClass ?? "card-footer";

                builder.OpenElement(9, "div");
                builder.AddAttribute(10, "class", footerClass);
                builder.AddContent(11, Footer);
                builder.CloseElement();
            }
            builder.CloseElement();
        }
    }
}
