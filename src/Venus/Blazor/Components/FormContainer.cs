using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class FormContainer : ControlBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        
        protected override void BuildClass(List<string> classList)
        {
            base.BuildClass(classList);

            classList.Add("form-container");
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", GetClass());
            builder.AddAttribute(1, "style", GetStyle());
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }
    }
}
