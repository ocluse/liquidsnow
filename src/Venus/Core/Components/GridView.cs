namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// An items control component that displays items in a grid.
/// </summary>
/// <typeparam name="T"></typeparam>
public class GridView<T> : CollectionView<T>, IGrid
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
    protected override void BuildContainerClass(ClassBuilder builder)
    {
        base.BuildContainerClass(builder);
        builder.Add("grid");
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add("grid-view");
    }

    ///<inheritdoc/>
    protected override void BuildContainerStyles(StyleBuilder builder)
    {
        base.BuildContainerStyles(builder);
        builder.AddAll(this.GetGridStyles());
    }
}
