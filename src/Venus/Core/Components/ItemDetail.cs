using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that that displays an item and its details rendered inside an <see cref="ItemDetailContainer"/>
/// </summary>
/// <remarks>
/// Renders a dt element with the item content and a dd element with the detail content.
/// </remarks>
public class ItemDetail : ControlBase
{
    /// <summary>
    /// Gets or sets the item to display.
    /// </summary>
    /// <remarks>
    /// When the <see cref="ItemContent"/> parameter is provided, this parameter is ignored.
    /// </remarks>
    [Parameter]
    public string? Item { get; set; }

    /// <summary>
    /// Gets or sets the item content to display.
    /// </summary>
    /// <remarks>
    /// This parameter takes precedence over the <see cref="Item"/> parameter.
    /// </remarks>
    [Parameter]
    public RenderFragment? ItemContent { get; set; }

    /// <summary>
    /// Gets or sets the detail content to display.
    /// </summary>
    /// <remarks>
    /// When the <see cref="ChildContent"/> parameter is provided, this parameter is ignored.
    /// </remarks>
    [Parameter]
    public string? Detail { get; set; }

    /// <summary>
    /// Gets or sets the child content to display.
    /// </summary>
    /// <remarks>
    /// This parameter takes precedence over the <see cref="Detail"/> parameter.
    /// </remarks>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (ItemContent != null)
        {
            builder.OpenElement(3, "dt");
            builder.AddMultipleAttributes(4, GetAttributes());
            builder.AddContent(5, ItemContent);
            builder.CloseElement();
        }
        else if (Item != null)
        {
            builder.OpenElement(0, "dt");
            builder.AddMultipleAttributes(1, GetAttributes());
            builder.AddContent(2, Item);
            builder.CloseElement();
        }

        if (ChildContent != null)
        {
            builder.OpenElement(6, "dd");
            builder.AddMultipleAttributes(7, GetAttributes());
            builder.AddContent(8, ChildContent);
            builder.CloseElement();
        }
        else if (Detail != null)
        {
            builder.OpenElement(3, "dd");
            builder.AddMultipleAttributes(4, GetAttributes());
            builder.AddContent(5, Detail);
            builder.CloseElement();
        }
    }
}
