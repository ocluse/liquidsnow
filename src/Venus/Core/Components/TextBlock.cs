using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that renders text.
/// </summary>
public class TextBlock : ControlBase
{
    /// <summary>
    /// Gets or sets the text style.
    /// </summary>
    [Parameter]
    public int TextStyle { get; set; } = Values.TextStyle.Body;

    /// <summary>
    /// Gets or sets the hierarchy that the text should be rendered as.
    /// </summary>
    [Parameter]
    public int Hierarchy { get; set; } = TextHierarchy.Span;

    /// <summary>
    /// Gets or sets the content of the text block.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, Resolver.ResolveTextHierarchy(Hierarchy));

        builder.AddMultipleAttributes(1, GetAttributes());

        if (ChildContent != null)
        {
            builder.AddContent(2, ChildContent);
        }

        builder.CloseElement();
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add(Resolver.ResolveTextStyle(TextStyle));
    }
}
