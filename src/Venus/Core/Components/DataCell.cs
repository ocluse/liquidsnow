using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that represents a data cell in a table, rendered as a td element.
/// </summary>
public class DataCell : ControlBase
{
    /// <summary>
    /// Gets or sets the content of the data cell.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "td");

        builder.AddMultipleAttributes(1, GetAttributes());
        builder.AddContent(2, ChildContent);
        builder.CloseElement();
    }
}
