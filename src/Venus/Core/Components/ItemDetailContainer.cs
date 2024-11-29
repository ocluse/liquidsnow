using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A container for displaying a collection of <see cref="ItemDetail"/> components.
/// </summary>
public class ItemDetailContainer : ControlBase
{
    /// <summary>
    /// Gets or sets the inner content of the container, which should be a collection of <see cref="ItemDetail"/> components.
    /// </summary>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "dl");
        {
            builder.AddMultipleAttributes(2, GetAttributes());
            builder.AddContent(3, ChildContent);
        }
        builder.CloseElement();
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder builder)
    {
        builder.Add(ClassNameProvider.ItemDetailContainer);
    }
}
