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
    /// Gets or sets the content displayed next to the checkbox.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the core html input element.
    /// </summary>
    [Parameter]
    public string? InputClass { get; set; }

    ///<inheritdoc/>
    protected override void BuildInputClass(ClassBuilder classBuilder)
    {
        base.BuildInputClass(classBuilder);
        classBuilder.Add(ClassNameProvider.CheckBox);
    }

    ///<inheritdoc/>
    protected override void BuildAttributes(IDictionary<string, object> attributes)
    {
        base.BuildAttributes(attributes);
        attributes.Add("for", NameAttributeValue);
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "label");
        {
            builder.AddMultipleAttributes(2, GetAttributes());

            if (Header.IsNotWhiteSpace())
            {
                builder.AddContent(3, Header);
            }
            else
            {
                builder.AddContent(4, ChildContent);
            }

            builder.OpenElement(5, "input");
            {
                if (InputClass.IsNotWhiteSpace())
                {
                    builder.AddAttribute(6, "class", InputClass);
                }

                builder.AddAttribute(7, "type", "checkbox");
                builder.AddAttribute(8, "name", NameAttributeValue);

                if (Id.IsNotEmpty())
                {
                    builder.AddAttribute(9, "id", Id);
                }

                builder.AddAttribute(10, "value", bool.TrueString);
                builder.AddAttribute(11, "checked", BindConverter.FormatValue(CurrentValue));
                builder.AddAttribute(12, "onchange", EventCallback.Factory.CreateBinder(this, value=>CurrentValue = value, CurrentValue));
                builder.SetUpdatesAttributeName("checked");

                if (Disabled)
                {
                    builder.AddAttribute(13, "disabled");
                }

                if (ReadOnly)
                {
                    builder.AddAttribute(14, "readonly");
                }
            }
            builder.CloseElement();

            builder.OpenElement(15, "span");
            {
                builder.AddAttribute(16, "class", ClassNameProvider.Checkbox_Checkmark);
            }
            builder.CloseElement();

        }
        builder.CloseElement();
    }

    ///<inheritdoc/>
    protected override bool TryParseValueFromString(string? value, [MaybeNullWhen(false)] out bool result, [NotNullWhen(false)] out string? validationErrorMessage)
    {
        throw new NotSupportedException();
    }
}
