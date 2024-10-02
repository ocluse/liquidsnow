using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Venus.Contracts;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The default container for forms.
/// </summary>
public class FormContainer : ControlBase, IForm
{
    /// <summary>
    /// Gets the inputs that are registered with the form.
    /// </summary>
    protected List<IInput> Inputs { get; } = [];

    /// <summary>
    /// The inner content of the form.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    ///<inheritdoc/>
    public virtual void Register(IInput input)
    {
        Inputs.Add(input);
    }

    ///<inheritdoc/>
    public virtual void Unregister(IInput input)
    {
        Inputs.Remove(input);
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);

        classBuilder.Add("form-container");
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, GetAttributes());
        builder.AddContent(2, ChildContent);
        builder.CloseElement();
    }
}
