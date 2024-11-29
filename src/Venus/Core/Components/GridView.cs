namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A collection component that renders items in a grid.
/// </summary>
public class GridView<T> : CollectionViewBase<T>, IGrid
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
    public CssUnit? GapUnit { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public double? ColumnGap { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public double? RowGap { get; set; }

    ///<inheritdoc/>
    public double? GapLg { get; set; }

    ///<inheritdoc/>
    public double? GapMd { get; set; }

    ///<inheritdoc/>
    public double? GapSm { get; set; }

    ///<inheritdoc/>
    public double? GapXs { get; set; }

    ///<inheritdoc/>
    protected override void BuildContainerClass(ClassBuilder builder)
    {
        base.BuildContainerClass(builder);
        builder.Add(ClassNameProvider.Grid);
    }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add(ClassNameProvider.GridView);
    }

    ///<inheritdoc/>
    protected override void BuildContainerStyles(StyleBuilder builder)
    {
        base.BuildContainerStyles(builder);
        builder.AddAll(this.GetGridStyles(Resolver));
    }
}
