using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Extensions;

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
            if (Id.IsNotEmpty())
            {
                builder.AddAttribute(2, "id", Id);
            }
            builder.AddAttribute(3, "placeholder", Placeholder ?? " ");
            builder.AddAttribute(4, "type", InputType);
            builder.AddAttribute(5, "name", AppliedName);
            builder.AddAttribute(6, "class", ClassBuilder.Join(ClassNameProvider.Field_Input, InputClass));
            builder.AddAttribute(7, "value", BindConverter.FormatValue(CurrentValue));
            builder.AddAttribute(8, "onchange", EventCallback.Factory.CreateBinder(this, value => CurrentValue = value, CurrentValue!));
            builder.SetUpdatesAttributeName("value");

            if (Disabled)
            {
                builder.AddAttribute(9, "disabled");
            }

            if (ReadOnly)
            {
                builder.AddAttribute(10, "readonly");
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
