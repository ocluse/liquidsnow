using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base class for elements that typically render a html input element.
/// </summary>
public abstract class TextBoxBase<TValue> : InputBase<TValue>
{
    /// <summary>
    /// Gets or sets the notification strategy for when the value of the input changes.
    /// </summary>
    [Parameter]
    public UpdateTrigger UpdateTrigger { get; set; }

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
    /// Gets or sets the CSS class applied to the input's content area.
    /// </summary>
    [Parameter]
    public string? ContentClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the prefix content containing div.
    /// </summary>
    [Parameter]
    public string? PrefixClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the suffix content containing div.
    /// </summary>
    [Parameter]
    public string? SuffixClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the header of the textbox.
    /// </summary>
    [Parameter]
    public string? HeaderClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the inner input element. 
    /// </summary>
    [Parameter]
    public string? InputClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the validation label of the input.
    /// </summary>
    [Parameter]
    public string? ValidationLabelClass { get; set; }

    /// <summary>
    /// Gets or sets the content to display in the header of the input.
    /// </summary>
    [Parameter]
    public RenderFragment<string>? HeaderContent { get; set; }

    /// <summary>
    /// Gets the type of value that the input accepts.
    /// </summary>
    protected virtual string InputType => "text";

    private async Task HandleValueUpdated(TValue? value)
    {
        ChangeEventArgs e = new()
        {
            Value = value
        };

        await HandleInputChange(e);
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
        classBuilder.Add(ClassNameProvider.TextBox);
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "div");
        {
            builder.AddMultipleAttributes(2, GetAttributes());

            //Header
            if (Header != null || HeaderContent != null)
            {
                string? headerClass = ClassBuilder.Join(ClassNameProvider.TextBox_Header, HeaderClass);

                if (HeaderContent != null)
                {
                    builder.OpenElement(3, "div");
                    {
                        builder.AddAttribute(4, "class", headerClass);
                        builder.AddContent(5, HeaderContent(AppliedName));
                    }
                    builder.CloseElement();
                }
                else
                {
                    builder.OpenElement(6, "label");
                    {
                        builder.AddAttribute(7, "class", headerClass);
                        builder.AddAttribute(8, "for", AppliedName);
                        builder.AddContent(9, Header);
                    }
                    builder.CloseElement();
                }
            }

            //The input content
            builder.OpenElement(10, "div");
            {
                builder.AddAttribute(11, "class", ClassBuilder.Join(ClassNameProvider.TextBox_Content, ContentClass));

                if (PrefixContent != null)
                {
                    builder.OpenElement(12, "div");
                    {
                        builder.AddAttribute(13, "class", ClassBuilder.Join(ClassNameProvider.TextBox_Prefix, PrefixClass));
                        builder.AddContent(14, PrefixContent);
                    }
                    builder.CloseElement();
                }

                builder.OpenRegion(15);
                {
                    BuildInput(builder);
                }
                builder.CloseRegion();

                if (SuffixContent != null)
                {
                    builder.OpenElement(16, "div");
                    {
                        builder.AddAttribute(17, "class", ClassBuilder.Join(ClassNameProvider.TextBox_Suffix, SuffixClass));
                        builder.AddContent(18, SuffixContent);
                    }
                    builder.CloseElement();
                }
            }
            builder.CloseElement();

            //Validation message
            if (ValidationContent != null)
            {
                builder.OpenElement(19, "div");
                {
                    builder.AddAttribute(20, "class", GetValidationClass());
                    builder.AddContent(21, ValidationContent(Validation));
                }
                builder.CloseElement();

            }
            else if (Validation != null && !string.IsNullOrEmpty(Validation.Message))
            {
                builder.OpenElement(22, "label");
                {
                    builder.AddAttribute(23, "class", GetValidationClass());
                    builder.AddAttribute(24, "role", "alert");
                    builder.AddContent(25, Validation.Message);
                }
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
        builder.OpenElement(0, "input");
        {
            builder.AddAttribute(1, "placeholder", Placeholder ?? " ");
            builder.AddAttribute(2, "type", InputType);
            builder.AddAttribute(3, "value", GetInputDisplayValue(Value));
            builder.AddAttribute(4, "class", ClassBuilder.Join(ClassNameProvider.TextBox_Input, InputClass));

            var valueUpdateCallback = EventCallback.Factory.CreateBinder(this, RuntimeHelpers.CreateInferredBindSetter(callback: HandleValueUpdated, value: Value), Value);

            builder.AddAttribute(5, UpdateTrigger.ToHtmlAttributeValue(), valueUpdateCallback);
            builder.SetUpdatesAttributeName("value");

            builder.AddAttribute(6, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, KeyDown));

            if (Disabled)
            {
                builder.AddAttribute(7, "disabled");
            }

            if (ReadOnly)
            {
                builder.AddAttribute(8, "readonly");
            }
        }
        builder.CloseElement();
    }

    /// <summary>
    /// Implemented by inheriting classes to convert the provided value to the input type.
    /// </summary>
    protected abstract TValue? GetValue(object? value);

    /// <summary>
    /// Bound to input element to handle value changes.
    /// </summary>
    protected async Task HandleInputChange(ChangeEventArgs e)
    {
        var newValue = GetValue(e.Value);
        await NotifyValueChange(newValue);
    }

    ///<inheritdoc/>
    protected override string GetValidationClass()
    {
        string baseClass = base.GetValidationClass();
        return ClassBuilder.Join(baseClass, ClassNameProvider.TextBox_ValidationLabel, ValidationLabelClass) ?? baseClass;
    }

    /// <summary>
    /// Gets the value to display for the input value.
    /// </summary>
    protected virtual object? GetInputDisplayValue(TValue? value)
    {
        return BindConverter.FormatValue(value);
    }
}
