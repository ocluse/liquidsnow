using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus.Components.Static;

/// <summary>
/// The static variant of the <see cref="CheckBox"/>
/// </summary>
public class StaticCheckBox : StaticInputBase<bool>
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
                    if (InputClass.IsNotWhiteSpace())
                    {
                        builder.AddAttribute(10, "class", InputClass);
                    }

                    builder.AddAttribute(11, "type", "checkbox");
                    builder.AddAttribute(12, "name", NameAttributeValue);

                    if (Id.IsNotEmpty())
                    {
                        builder.AddAttribute(13, "id", Id);
                    }

                    builder.AddAttribute(14, "value", bool.TrueString);
                    builder.AddAttribute(15, "checked", BindConverter.FormatValue(CurrentValue));
                    builder.AddAttribute(16, "onchange", EventCallback.Factory.CreateBinder(this, value => CurrentValue = value, CurrentValue));
                    builder.SetUpdatesAttributeName("checked");

                    if (Disabled)
                    {
                        builder.AddAttribute(17, "disabled");
                    }

                    if (ReadOnly)
                    {
                        builder.AddAttribute(18, "readonly");
                    }
                }
                builder.CloseElement();

                builder.OpenElement(19, "span");
                {
                    builder.AddAttribute(20, "class", ClassNameProvider.CheckBox_Checkmark);
                }
                builder.CloseElement();

            }
            builder.CloseElement();

            bool alwaysRenderValidation = AlwaysRenderValidationLabel ?? Resolver.AlwaysRenderCheckBoxValidationLabel;

            ValidationResult? validation = GetValidationResult();

            if (validation != null || alwaysRenderValidation)
            {
                //Validation message
                if (ValidationContent != null)
                {
                    builder.OpenElement(21, "div");
                    {
                        builder.AddAttribute(22, "class", GetValidationClass());
                        builder.AddContent(23, ValidationContent(validation));
                    }
                    builder.CloseElement();

                }
                else
                {
                    builder.OpenElement(24, "label");
                    {
                        builder.AddAttribute(25, "class", GetValidationClass());
                        builder.AddAttribute(26, "role", "alert");
                        if (validation?.Message.IsNotEmpty() == true)
                        {
                            builder.AddContent(27, validation.Message);
                        }
                    }
                    builder.CloseElement();
                }
            }
        }
        builder.CloseElement();
    }

    ///<inheritdoc/>
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out bool result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        throw new NotSupportedException();
    }
}
