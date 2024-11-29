using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components.Static;

/// <summary>
/// The static variant of the <see cref="TextBoxBase{TValue}"/>
/// </summary>
public abstract class StaticTextBoxBase<TValue> : StaticFieldBase<TValue>
{
    /// <summary>
    /// Gets the type of value that the input accepts.
    /// </summary>
    protected virtual string InputType => "text";

    ///<inheritdoc/>
    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "input");
        {
            builder.AddAttribute(2, "placeholder", Placeholder ?? " ");
            builder.AddAttribute(3, "type", InputType);
            builder.AddAttribute(4, "name", AppliedName);
            builder.AddAttribute(5, "class", ClassBuilder.Join(ClassNameProvider.Field_Input, InputClass));
            builder.AddAttribute(6, "value", BindConverter.FormatValue(CurrentValue));
            builder.AddAttribute(7, "onchange", EventCallback.Factory.CreateBinder(this, value => CurrentValue = value, CurrentValue!));
            builder.SetUpdatesAttributeName("value");

            if (Disabled)
            {
                builder.AddAttribute(8, "disabled");
            }

            if (ReadOnly)
            {
                builder.AddAttribute(9, "readonly");
            }
        }
        builder.CloseElement();
    }

    ///<inheritdoc/>
    protected override void BuildInputClass(ClassBuilder builder)
    {
        base.BuildInputClass(builder);
        builder.Add(ClassNameProvider.TextBox);
    }

}
