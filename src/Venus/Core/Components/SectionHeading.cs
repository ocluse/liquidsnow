using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that represents a section heading, rendered as an h2 element.
/// </summary>
public class SectionHeading : ControlBase
{
    /// <summary>
    /// The content of the section heading.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public RenderFragment? ChildContent { get; set; }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<TextBlock>(0);
        builder.AddAttribute(1, nameof(TextBlock.Hierarchy), TextHierarchy.H2);
        builder.AddMultipleAttributes(2, GetAttributes());
        builder.AddAttribute(3, nameof(TextBlock.TextStyle), TextStyle.Subtitle);
        builder.AddAttribute(4, nameof(TextBlock.ChildContent), ChildContent);
        builder.CloseComponent();
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add("heading");
    }
}
