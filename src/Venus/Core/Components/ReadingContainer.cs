using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A type of component that is appropriate for displaying articles.
/// </summary>
public class ReadingContainer : ControlBase
{
    /// <summary>
    /// Gets or sets the inner content of the container.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Gets or sets the name of the element to render.
    /// </summary>
    [Parameter]
    public string ElementName { get; set; } = "div";

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add("reading-container");
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, ElementName);

        builder.AddMultipleAttributes(1, GetAttributes());

        if (ChildContent != null)
        {
            builder.AddContent(2, ChildContent);
        }

        builder.CloseElement();
    }
}
