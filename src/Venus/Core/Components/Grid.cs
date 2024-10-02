using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that arranges its children in a grid.
/// </summary>
public class Grid : ControlBase, IGrid
{
    ///<inheritdoc/>
    [Parameter]
    public int Columns { get; set; } = 1;

    ///<inheritdoc/>
    [Parameter]
    public int? ColumnsLg { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public int? ColumnsMd { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public int? ColumnsSm { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public int? ColumnsXs { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public double Gap { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public double? ColumnGap { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public double? RowGap { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");

        builder.AddMultipleAttributes(1, GetAttributes());

        BuildContent(builder);

        builder.CloseElement();
    }

    /// <summary>
    /// Build the content of the grid.
    /// </summary>
    protected virtual void BuildContent(RenderTreeBuilder builder)
    {
        if (ChildContent != null)
        {
            builder.AddContent(2, ChildContent);
        }
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add("grid");
    }

    ///<inheritdoc/>
    protected override void BuildStyle(StyleBuilder styleBuilder)
    {
        base.BuildStyle(styleBuilder);
        styleBuilder.AddAll(this.GetGridStyles());
    }
}
