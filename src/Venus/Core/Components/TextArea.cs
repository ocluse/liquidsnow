using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that allows a user to input multiple lines of text.
/// </summary>
public class TextArea : TextBoxBase<string>
{
    ///<inheritdoc/>
    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "textarea");
        {
            builder.AddAttribute(2, "placeholder", Placeholder ?? " ");
            builder.AddAttribute(3, "class", ClassBuilder.Join(ClassNameProvider.Field_Input, InputClass));
            builder.AddAttribute(4, "name", AppliedName);
            builder.AddAttribute(5, "onchange", EventCallback.Factory.CreateBinder(this, HandleValueUpdated, Value ?? string.Empty));
            builder.AddContent(6, Value);
        }
        builder.CloseElement();
    }

    ///<inheritdoc/>
    protected override string GetValue(object? value)
    {
        return value?.ToString() ?? string.Empty;
    }

    ///<inheritdoc/>
    protected override void BuildInputClass(ClassBuilder classBuilder)
    {
        //Don't call base to prevent the 'textbox' class from being added.
        classBuilder.Add(ClassNameProvider.TextArea);
    }

    private async Task HandleValueUpdated(string? value)
    {
        ChangeEventArgs e = new()
        {
            Value = value
        };

        await HandleInputChange(e);
    }
}
