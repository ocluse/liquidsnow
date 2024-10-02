using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// Represents a checkbox input component for selecting a boolean value.
/// </summary>
public class CheckBox : InputBase<bool>
{
    /// <summary>
    /// The content displayed next to the checkbox.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the inner input element
    /// </summary>
    [Parameter]
    public string? InputClass { get; set; }

    ///<inheritdoc/>
    protected override void BuildInputClass(ClassBuilder classBuilder)
    {
        base.BuildInputClass(classBuilder);
        classBuilder.Add("checkbox");
    }

    ///<inheritdoc/>
    protected override void BuildAttributes(IDictionary<string, object> attributes)
    {
        base.BuildAttributes(attributes);
        attributes.Add("for", AppliedName);
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "label");
        {
            builder.AddMultipleAttributes(1, GetAttributes());

            if (string.IsNullOrEmpty(Header))
            {
                builder.AddContent(3, ChildContent);
            }
            else
            {
                builder.AddContent(4, Header);
            }

            builder.OpenElement(5, "input");
            {
                builder.AddAttribute(6, "type", "checkbox");
                builder.AddAttribute(7, "onchange", OnChange);
                builder.AddAttribute(8, "checked", Value);
                builder.AddAttribute(9, "name", AppliedName);

                if (!string.IsNullOrWhiteSpace(InputClass))
                {
                    builder.AddAttribute(10, "class", InputClass);
                }

                if (Disabled)
                {
                    builder.AddAttribute(11, "disabled");
                }

                if (ReadOnly)
                {
                    builder.AddAttribute(12, "readonly");
                }
            }
            builder.CloseElement();

            builder.OpenElement(13, "span");
            {
                builder.AddAttribute(14, "class", "checkmark");
            }
            builder.CloseElement();

        }
        builder.CloseElement();
    }

    ///<inheritdoc/>
    protected override bool GetValue(object? value)
    {
        if (bool.TryParse(value?.ToString(), out bool result))
        {
            return result;
        }
        else
        {
            return false;
        }
    }
}
