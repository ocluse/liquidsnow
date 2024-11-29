using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components.Static;

/// <summary>
/// A convenience component for submitting a form.
/// </summary>
public class StaticSubmit : ControlBase
{
    /// <summary>
    /// Gets or sets the text displayed on the submit button.
    /// </summary>
    [Parameter]
    public string? Text { get; set; }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "input");
        {
            builder.AddAttribute(2, "type", "submit");
            builder.AddAttribute(3, "value", Text);
            builder.AddMultipleAttributes(4, GetAttributes());
        }
        builder.CloseElement();
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder builder)
    {
        base.BuildClass(builder);
        builder.Add(ClassNameProvider.SubmitButton);
    }
}
