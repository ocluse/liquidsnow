using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Ocluse.LiquidSnow.Venus.Models;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A type of <see cref="InputControlBase{T}"/> that renders a html 'input' element, or something similar such as a dropdown.
/// </summary>
public abstract class InputControlBase<T> : InputBase<T>
{
    /// <summary>
    /// Gets or sets the notification strategy for when the value of the input changes.
    /// </summary>
    [Parameter]
    public UpdateTrigger UpdateTrigger { get; set; } = UpdateTrigger.OnChange;

    /// <summary>
    /// Gets or sets a callback that will be invoked when a key is pressed down while the input has focus.
    /// </summary>
    [Parameter]
    public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

    /// <summary>
    /// Gets or sets a callback that will be invoked when the 'Enter' key is pressed while the input has focus.
    /// </summary>
    [Parameter]
    public EventCallback OnReturn { get; set; }

    /// <summary>
    /// Gets or sets the classes that are applied to various children of the input.
    /// </summary>
    [Parameter]
    public InputClasses? InputClasses { get; set; }

    /// <summary>
    /// The type of value that the input accepts.
    /// </summary>
    protected virtual string InputType => "text";

    private async Task HandleValueUpdated(T? value)
    {
        ChangeEventArgs e = new()
        {
            Value = value
        };

        await OnChange(e);
    }

    private async Task KeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" || e.Code == "NumpadEnter")
        {
            await OnReturn.InvokeAsync();
        }
        await OnKeyDown.InvokeAsync(e);
    }

    ///<inheritdoc/>
    protected override void BuildInputClass(ClassBuilder classBuilder)
    {
        classBuilder.Add("textbox");
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, GetAttributes());

        //The input content
        builder.OpenElement(1, "div");
        {
            builder.AddAttribute(2, "class", ClassBuilder.Join("input_main", InputClasses?.ContentArea));

            if (PrefixContent != null)
            {
                builder.OpenElement(3, "div");
                builder.AddAttribute(4, "class", ClassBuilder.Join("input_prefix", InputClasses?.PrefixContent));
                builder.AddContent(5, PrefixContent);
                builder.CloseElement();
            }

            BuildInput(builder);

            if (SuffixContent != null)
            {
                builder.OpenElement(6, "div");
                builder.AddAttribute(7, "class", ClassBuilder.Join("input_suffix", InputClasses?.SuffixContent));
                builder.AddContent(8, SuffixContent);
                builder.CloseElement();
            }

            //Header-label
            if (Header != null)
            {
                builder.OpenElement(50, "label");
                builder.AddAttribute(51, "class", ClassBuilder.Join("input_header", InputClasses?.HeaderLabel));
                builder.AddContent(52, Header);
                builder.CloseElement();
            }
        }
        builder.CloseElement();

        //Validation message
        if (ValidationContent != null)
        {
            builder.AddContent(53, ValidationContent);
        }
        else
        {
            if (Validation != null && !string.IsNullOrEmpty(Validation.Message))
            {
                builder.OpenElement(55, "label");
                builder.AddAttribute(56, "class", GetValidationClass());
                builder.AddAttribute(57, "role", "alert");
                builder.AddContent(58, Validation.Message);
                builder.CloseElement();
            }
        }


        builder.CloseElement();
    }

    /// <summary>
    /// Build the actual input element.
    /// </summary>
    protected virtual void BuildInput(RenderTreeBuilder builder)
    {
        builder.OpenElement(10, "input");
        builder.AddAttribute(13, "placeholder", Placeholder ?? " ");
        builder.AddAttribute(14, "type", InputType);
        builder.AddAttribute(15, "value", GetInputDisplayValue(Value));
        builder.AddAttribute(16, "class", ClassBuilder.Join("input_input", InputClasses?.Input));

        var valueUpdateCallback = EventCallback.Factory.CreateBinder(this, RuntimeHelpers.CreateInferredBindSetter(callback: HandleValueUpdated, value: Value), Value);

        builder.AddAttribute(15, UpdateTrigger.ToHtmlAttributeKey(), valueUpdateCallback);
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

    

    ///<inheritdoc/>
    protected override string GetValidationClass()
    {
        string baseClass = base.GetValidationClass();
        return ClassBuilder.Join(baseClass, "input_validation") ?? baseClass;
    }

    /// <summary>
    /// Gets the value to display for the input value.
    /// </summary>
    protected virtual object? GetInputDisplayValue(T? value)
    {
        return BindConverter.FormatValue(value);
    }
}
