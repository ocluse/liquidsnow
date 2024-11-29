using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that renders a td element.
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
        builder.OpenElement(1, "td");
        {
            builder.AddMultipleAttributes(2, GetAttributes());
            builder.AddContent(3, ChildContent);
        }
        builder.CloseElement();
    }
}
