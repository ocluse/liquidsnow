using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that represents a header cell in a table, rendered as a th element.
/// </summary>
public class HeaderCell : ControlBase
{
    /// <summary>
    /// Gets or sets the content of the header cell.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "th");

        builder.AddMultipleAttributes(1, GetAttributes());
        builder.AddContent(2, ChildContent);
        builder.CloseElement();
    }
}
