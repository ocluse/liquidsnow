using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A collection view component that renders items in a table.
/// </summary>
public class TableView<T> : CollectionView<T>
{
    ///<inheritdoc/>
    protected override string ItemElement => "tr";

    ///<inheritdoc/>
    protected override string ContainerElement => "table";

    /// <summary>
    /// Gets or sets the template for the header of the table.
    /// </summary>
    [Parameter]
    public RenderFragment? HeaderTemplate { get; set; }

    /// <summary>
    /// Gets or sets the template for the footer of the table.
    /// </summary>
    [Parameter]
    public RenderFragment? FooterTemplate { get; set; }

    ///<inheritdoc/>
    protected override void BuildContainerClass(ClassBuilder builder)
    {
        base.BuildContainerClass(builder);
        builder.Add("table");
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add("table-view");
    }

    ///<inheritdoc/>
    protected override void RenderItems(RenderTreeBuilder builder, IEnumerable<T> items)
    {
        if (HeaderTemplate != null)
        {
            builder.OpenElement(30, "thead");
            builder.AddContent(31, HeaderTemplate);
            builder.CloseElement();
        }

        builder.OpenElement(32, "tbody");
        base.RenderItems(builder, items);
        builder.CloseElement();

        if (FooterTemplate != null)
        {
            builder.OpenElement(33, "tfoot");
            builder.AddContent(34, FooterTemplate);
            builder.CloseElement();
        }
    }
}
