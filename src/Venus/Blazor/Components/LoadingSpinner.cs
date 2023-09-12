using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class LoadingSpinner : ControlBase
    {
        protected override void BuildClass(List<string> classList)
        {
            base.BuildClass(classList);
            classList.Add("loading-spinner");
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", GetClass());
            builder.AddAttribute(2, "style", GetStyle());
            builder.CloseElement();
        }
    }
}
