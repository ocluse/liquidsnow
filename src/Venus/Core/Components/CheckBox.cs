using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// An input for collecting boolean values.
/// </summary>
public class CheckBox : InputBase<bool>
{
    /// <summary>
    /// Gets or sets the ID of the checkbox.
    /// </summary>
    [Parameter]
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the content displayed next to the checkbox.
    /// </summary>
    [Parameter]
    public RenderFragment<bool>? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the core html input element.
    /// </summary>
    [Parameter]
    public string? InputClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to be applied to the content of the checkbox.
    /// </summary>
    [Parameter]
    public string? ContentClass { get; set; }

    /// <summary>
    /// Gets or sets whether the validation label should always be rendered even without a validation message.
    /// </summary>
    [Parameter]
    public bool? AlwaysRenderValidationLabel { get; set; }

    private async Task HandleInputChange(ChangeEventArgs e)
    {
        if (!bool.TryParse(e.Value?.ToString(), out bool newValue))
        {
            newValue = false;
        }
        await NotifyValueChange(newValue);
    }

    ///<inheritdoc/>
    protected override void BuildInputClass(ClassBuilder classBuilder)
    {
        base.BuildInputClass(classBuilder);
        classBuilder.Add(ClassNameProvider.CheckBox);
    }

    ///<inheritdoc/>
    protected override string? GetValidationClass()
    {
        string? baseClass = base.GetValidationClass();
        return ClassBuilder.Join(baseClass, ClassNameProvider.CheckBox_ValidationLabel, ValidationLabelClass) ?? baseClass;
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "div");
        {
            builder.AddMultipleAttributes(2, GetAttributes());

            builder.OpenElement(3, "label");
            {
                builder.AddAttribute(4, "class", ClassBuilder.Join(ClassNameProvider.CheckBox_Content, ContentClass));
                
                if (Id.IsNotEmpty())
                {
                    builder.AddAttribute(5, "for", Id);
                }

                if (ChildContent != null)
                {
                    builder.AddContent(6, ChildContent(Value));
                }
                else
                {
                    builder.OpenElement(7, "span");
                    {
                        builder.AddContent(8, Header);
                    }
                    builder.CloseElement();
                }

                builder.OpenElement(9, "input");
                {
                    if (Id.IsNotEmpty())
                    {
                        builder.AddAttribute(10, "class", InputClass);
                    }
                    builder.AddAttribute(11, "type", "checkbox");
                    builder.AddAttribute(12, "onchange", EventCallback.Factory.Create(this, HandleInputChange));
                    builder.AddAttribute(13, "checked", Value);
                    builder.AddAttribute(14, "name", AppliedName);

                    if (InputClass.IsNotWhiteSpace())
                    {
                        builder.AddAttribute(15, "class", InputClass);
                    }

                    if (Disabled)
                    {
                        builder.AddAttribute(16, "disabled");
                    }

                    if (ReadOnly)
                    {
                        builder.AddAttribute(17, "readonly");
                    }
                }
                builder.CloseElement();

                builder.OpenElement(18, "span");
                {
                    builder.AddAttribute(19, "class", ClassNameProvider.CheckBox_Checkmark);
                }
                builder.CloseElement();

                
            }
            builder.CloseElement();

            bool alwaysRenderValidation = AlwaysRenderValidationLabel ?? Resolver.AlwaysRenderCheckBoxValidationLabel;

            if (Validation != null || alwaysRenderValidation)
            {
                //Validation message
                if (ValidationContent != null)
                {
                    builder.OpenElement(20, "div");
                    {
                        builder.AddAttribute(21, "class", GetValidationClass());
                        builder.AddContent(22, ValidationContent(Validation));
                    }
                    builder.CloseElement();

                }
                else
                {
                    builder.OpenElement(23, "label");
                    {
                        builder.AddAttribute(24, "class", GetValidationClass());
                        builder.AddAttribute(25, "role", "alert");
                        if (Validation?.Message.IsNotEmpty() == true)
                        {
                            builder.AddContent(26, Validation.Message);
                        }
                    }
                    builder.CloseElement();
                }
            }
        }
        builder.CloseElement();
    }
}
