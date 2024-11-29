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
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the core html input element.
    /// </summary>
    [Parameter]
    public string? InputClass { get; set; }

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
    protected override void BuildAttributes(IDictionary<string, object> attributes)
    {
        base.BuildAttributes(attributes);
        if (Id.IsNotEmpty())
        {
            attributes.Add("for", Id);
        }
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
                if (Id.IsNotEmpty())
                {
                    builder.AddAttribute(6, "class", InputClass);
                }
                builder.AddAttribute(7, "type", "checkbox");
                builder.AddAttribute(8, "onchange", EventCallback.Factory.Create(this, HandleInputChange));
                builder.AddAttribute(9, "checked", Value);
                builder.AddAttribute(10, "name", AppliedName);

                if (InputClass.IsNotWhiteSpace())
                {
                    builder.AddAttribute(11, "class", InputClass);
                }

                if (Disabled)
                {
                    builder.AddAttribute(12, "disabled");
                }

                if (ReadOnly)
                {
                    builder.AddAttribute(13, "readonly");
                }
            }
            builder.CloseElement();

            builder.OpenElement(14, "span");
            {
                builder.AddAttribute(15, "class", ClassNameProvider.Checkbox_Checkmark);
            }
            builder.CloseElement();

        }
        builder.CloseElement();
    }
}
