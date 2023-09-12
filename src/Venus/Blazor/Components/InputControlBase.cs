using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public abstract class InputControlBase<T> : InputBase<T>
    {
        [Parameter]
        public UpdateTrigger UpdateTrigger { get; set; } = UpdateTrigger.OnChange;

        protected override void BuildInputClass(List<string> classList)
        {
            classList.Add("textbox");
        }

        

        private string GetUpdateTrigger()
        {
            return UpdateTrigger switch
            {
                UpdateTrigger.OnChange => "onchange",
                UpdateTrigger.OnInput => "oninput",
                _ => throw new NotImplementedException()
            };
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");

            builder.AddAttribute(1, "class", GetClass());



            //Input itself
            BuildInput(builder);

            //Floating label
            if (Header != null)
            {
                builder.OpenElement(50, "span");
                builder.AddAttribute(51, "class", "header-label");
                builder.AddContent(52, Header);
                builder.CloseElement();
            }

            //Validation message
            if (!string.IsNullOrEmpty(ValidationResult.Message))
            {
                builder.OpenElement(53, "span");
                builder.AddAttribute(54, "class", GetValidationClass());
                builder.AddContent(55, ValidationResult.Message);
                builder.CloseElement();
            }

            builder.CloseElement();
        }

        protected virtual void BuildInput(RenderTreeBuilder builder)
        {
            builder.OpenElement(10, "input");
            builder.AddAttribute(13, "placeholder", Placeholder ?? " ");
            builder.AddAttribute(14, "type", GetInputType());
            builder.AddAttribute(15, GetUpdateTrigger(), OnChange);
            builder.AddAttribute(16, "value", GetInputDisplayValue(Value));

            if (Disabled)
            {
                builder.AddAttribute(17, "disabled");
            }

            if(ReadOnly)
            {
                builder.AddAttribute(18, "readonly");
            }
            
            builder.CloseElement();
        }

        protected virtual string GetInputType()
        {
            return "text";
        }

        protected virtual object? GetInputDisplayValue(T? value)
        {
            return value;
        }
    }
}
