using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that represents a button.
/// </summary>
public class Button : ButtonBase
{
    /// <summary>
    /// The content displayed in the bounds of the button
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    ///<inheritdoc/>
    protected override void BuildButtonClass(ClassBuilder classBuilder)
    {
        base.BuildButtonClass(classBuilder);
        classBuilder.Add("button");
    }

    ///<inheritdoc/>
    protected override void BuildContent(RenderTreeBuilder builder)
    {
        builder.AddContent(2, ChildContent);
    }
}
