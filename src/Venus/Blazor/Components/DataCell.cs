using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class DataCell : ControlBase
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "td");

            Dictionary<string, object> attributes = new()
            {
                { "class", GetClass() },
                {"style", GetStyle() }
            };

            builder.AddMultipleAttributes(1, attributes);
            builder.AddContent(2, ChildContent);
            builder.CloseElement();
        }
    }
}
