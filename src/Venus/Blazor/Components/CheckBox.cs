using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class CheckBox : InputBase<bool>
    {
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        protected override void BuildInputClass(List<string> classList)
        {
            base.BuildInputClass(classList);
            classList.Add("checkbox");
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "label");
            builder.AddAttribute(1, "class", GetClass());
            builder.AddAttribute(2, "style", GetStyle());

            if (string.IsNullOrEmpty(Header))
            {
                builder.AddContent(3, ChildContent);
            }
            else
            {
                builder.AddContent(4, Header);
            }

            builder.OpenElement(5, "input");
            builder.AddAttribute(6, "type", "checkbox");
            builder.AddAttribute(7, "onchange", OnChange);
            builder.AddAttribute(8, "checked", Value);

            if (Disabled)
            {
                builder.AddAttribute(9, "disabled");
            }

            if (ReadOnly)
            {
                builder.AddAttribute(10, "readonly");
            }
            builder.CloseElement();

            builder.OpenElement(9, "span");
            builder.AddAttribute(10, "class", "checkmark");
            builder.CloseElement();

            builder.CloseElement();
        }

        protected override bool GetValue(object? value)
        {
            if(bool.TryParse(value?.ToString(), out bool result))
            {
                return result;
            }
            else
            {
                return false;
            }
        }
    }
}
