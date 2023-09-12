using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class IconText : ControlBase
    {
        [Parameter]
        public string? Icon { get; set; }


        [Parameter]
        public int IconSize { get; set; } = DefaultSize.Size18;

        [Parameter]
        public int? IconColor { get; set; }

        [Parameter]
        public int TextStyle { get; set; } = Values.TextStyle.Body;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            Dictionary<string, object> attributes = new()
            {
                { "class", GetClass() },
                {"style", GetStyle() },
            };
            builder.AddMultipleAttributes(1, attributes);
            if (Icon != null)
            {
                builder.OpenComponent<FeatherIcon>(2);
                builder.AddAttribute(3, "Icon", Icon);
                builder.AddAttribute(4, "Size", IconSize);

                if (IconColor != null)
                {
                    builder.AddAttribute(5, "Color", IconColor);
                }

                builder.CloseComponent();
            }
            if (ChildContent != null)
            {
                builder.OpenComponent<TextBlock>(6);
                //builder.AddAttribute(7, nameof(Color), Color);
                builder.AddAttribute(8, nameof(TextBlock.TextStyle), TextStyle);
                builder.AddAttribute(9, nameof(TextBlock.ChildContent), ChildContent);
                builder.CloseComponent();
            }
            builder.CloseElement();
        }

        protected override void BuildClass(List<string> classList)
        {
            base.BuildClass(classList);
            classList.Add("icon-text");
        }
    }
}
