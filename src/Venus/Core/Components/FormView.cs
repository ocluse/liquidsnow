using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component for hosting form controls.
/// </summary>
public class FormView : ControlBase, IForm
{
    /// <summary>
    /// Gets the inputs that are registered with the form.
    /// </summary>
    protected List<IFormControl> Inputs { get; } = [];

    /// <summary>
    /// Gets or sets the inner content of the form.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    ///<inheritdoc/>
    public virtual void Register(IFormControl input)
    {
        Inputs.Add(input);
    }

    ///<inheritdoc/>
    public virtual void Unregister(IFormControl input)
    {
        Inputs.Remove(input);
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);

        classBuilder.Add(ClassNameProvider.FormView);
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        {
            builder.AddMultipleAttributes(1, GetAttributes());
            builder.AddContent(2, ChildContent);
        }
        builder.CloseElement();
    }
}
