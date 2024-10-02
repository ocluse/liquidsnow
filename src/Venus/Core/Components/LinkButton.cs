using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A button that is rendered as a link.
/// </summary>
public class LinkButton : ButtonBase
{
    /// <summary>
    /// Gets or sets the text style of the link.
    /// </summary>
    [Parameter]
    public int TextStyle { get; set; } = Values.TextStyle.Body;

    /// <summary>
    /// Gets or sets the hierarchy of the text.
    /// </summary>
    [Parameter]
    public int Hierarchy { get; set; } = TextHierarchy.Span;

    /// <summary>
    /// Gets or sets the content of the link text.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    ///<inheritdoc/>
    protected override void BuildButtonClass(ClassBuilder classBuilder)
    {
        base.BuildButtonClass(classBuilder);
        classBuilder.Add("link");
        if (Hierarchy == TextHierarchy.Span)
        {
            classBuilder.Add($"text-{TextStyle.ToString().PascalToKebabCase()}");
        }
    }

    ///<inheritdoc/>
    protected override void BuildContent(RenderTreeBuilder builder)
    {
        if (Hierarchy == TextHierarchy.Span)
        {
            builder.AddContent(2, ChildContent);
        }
        else
        {
            builder.OpenComponent<TextBlock>(2);
            builder.AddAttribute(3, nameof(TextBlock.Hierarchy), Hierarchy);
            builder.AddAttribute(4, nameof(TextBlock.TextStyle), TextStyle);
            builder.AddAttribute(5, nameof(TextBlock.ChildContent), ChildContent);
            builder.CloseComponent();
        }
    }
}
