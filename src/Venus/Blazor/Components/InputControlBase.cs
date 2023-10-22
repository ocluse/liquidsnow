using Microsoft.AspNetCore.Components.CompilerServices;
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
                builder.OpenElement(50, "label");
                builder.AddAttribute(51, "class", "header-label");
                builder.AddContent(52, Header);
                builder.CloseElement();
            }

            //Validation message
            if (Validation != null && !string.IsNullOrEmpty(Validation.Value.Message))
            {
                builder.OpenElement(53, "span");
                builder.AddAttribute(54, "class", GetValidationClass());
                builder.AddAttribute(55, "role", "alert");
                builder.AddContent(56, Validation.Value.Message);
                builder.CloseElement();
            }

            builder.CloseElement();
        }

        protected virtual void BuildInput(RenderTreeBuilder builder)
        {
            builder.OpenElement(10, "input");
            builder.AddAttribute(13, "placeholder", Placeholder ?? " ");
            builder.AddAttribute(14, "type", InputType);
            builder.AddAttribute(16, "value", GetInputDisplayValue(Value));

            var valueUpdateCallback = EventCallback.Factory.CreateBinder(this, RuntimeHelpers.CreateInferredBindSetter(callback: HandleValueUpdated, value: Value), Value);

            builder.AddAttribute(15, GetUpdateTrigger(), valueUpdateCallback);
            builder.SetUpdatesAttributeName("value");

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

        private async Task HandleValueUpdated(T? value)
        {
            ChangeEventArgs e = new()
            {
                Value = value
            };

            await OnChange(e);
        }

        protected virtual object? GetInputDisplayValue(T? value)
        {
            return BindConverter.FormatValue(value);
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
