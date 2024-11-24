using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// An input for collecting boolean values.
/// </summary>
public class CheckBox : InputBase<bool>
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

    private async Task OnInputChange(ChangeEventArgs e)
    {
        var newValue = bool.Parse(e.Value?.ToString() ?? "false");
        await NotifyValueChange(newValue);
    }

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
        attributes.Add("for", AppliedName);
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
                builder.AddAttribute(6, "type", "checkbox");
                builder.AddAttribute(7, "onchange", OnInputChange);
                builder.AddAttribute(8, "checked", Value);
                builder.AddAttribute(9, "name", AppliedName);

                if (InputClass.IsNotWhiteSpace())
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
                builder.AddAttribute(14, "class", ClassNameProvider.Checkbox_Checkmark);
            }
            builder.CloseElement();

        }
        builder.CloseElement();
    }
}
