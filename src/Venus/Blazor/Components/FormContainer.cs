using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class FormContainer : ControlBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        
        protected override void BuildClass(ClassBuilder classBuilder)
        {
            base.BuildClass(classBuilder);

            classBuilder.Add("form-container");
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddMultipleAttributes(1, GetClassAndStyle());
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }
    }
}
