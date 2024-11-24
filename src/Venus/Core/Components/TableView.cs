using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A collection view component that renders items in a table.
/// </summary>
public class TableView<T> : CollectionViewBase<T>
{
    ///<inheritdoc/>
    protected override string ItemElement => "tr";

    ///<inheritdoc/>
    protected override string ContainerElement => "table";

    /// <summary>
    /// Gets or sets the template for the header of the table rendered as the thead element.
    /// </summary>
    [Parameter]
    public RenderFragment? TableHeader { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the thead element.
    /// </summary>
    [Parameter]
    public string? TableHeaderClass { get; set; }

    /// <summary>
    /// Gets or sets the template for the footer of the table rendered as the tfoot element.
    /// </summary>
    [Parameter]
    public RenderFragment? TableFooter { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the tfoot element.
    /// </summary>
    [Parameter]
    public string? TableFooterClass { get; set; }

    ///<inheritdoc/>
    protected override void BuildContainerClass(ClassBuilder builder)
    {
        base.BuildContainerClass(builder);
        builder.Add(ClassNameProvider.TableView_Container);
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add(ClassNameProvider.TableView);
    }

    ///<inheritdoc/>
    protected override void BuildItems(RenderTreeBuilder builder, IEnumerable<T> items)
    {
        if (TableHeader != null)
        {
            builder.OpenElement(1, "thead");
            {
                if (TableHeaderClass != null)
                {
                    builder.AddAttribute(2, "class", TableHeaderClass);
                }
                builder.AddContent(3, TableHeader);
            }
            builder.CloseElement();
        }

        builder.OpenElement(4, "tbody");
        {
            builder.OpenRegion(5);
            {
                base.BuildItems(builder, items);
            }
            builder.CloseRegion();
        }
        builder.CloseElement();

        if (TableFooter != null)
        {
            builder.OpenElement(6, "tfoot");
            {
                if (TableFooterClass != null)
                {
                    builder.AddAttribute(7, "class", TableFooterClass);
                }
                builder.AddContent(8, TableFooter);
            }
            builder.CloseElement();
        }
    }
}
