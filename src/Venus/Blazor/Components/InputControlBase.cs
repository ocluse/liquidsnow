using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public abstract class InputControlBase<T> : InputBase<T>
    {
        [Parameter]
        public UpdateTrigger UpdateTrigger { get; set; } = UpdateTrigger.OnChange;

        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        [Parameter]
        public EventCallback OnReturn { get; set; }

        protected virtual string InputType { get; } = "text";

        protected override void BuildInputClass(ClassBuilder classBuilder)
        {
            classBuilder.Add("textbox");
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

            builder.AddMultipleAttributes(1, GetClassAndStyle());

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
            builder.AddAttribute(14, "type", InputType);
            builder.AddAttribute(15, GetUpdateTrigger(), OnChange);
            builder.AddAttribute(16, "value", GetInputDisplayValue(Value));
            builder.AddAttribute(17, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, KeyDown));
            if (Disabled)
            {
                builder.AddAttribute(18, "disabled");
            }

            if (ReadOnly)
            {
                builder.AddAttribute(19, "readonly");
            }

            builder.CloseElement();
        }

        protected virtual object? GetInputDisplayValue(T? value)
        {
            return value;
        }

        private async Task KeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" || e.Code == "NumpadEnter")
            {
                await OnReturn.InvokeAsync();
            }
            await OnKeyDown.InvokeAsync(e);
        }
    }
}
