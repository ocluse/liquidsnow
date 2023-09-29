namespace Ocluse.LiquidSnow.Venus.Blazor.Components;

public class GridView<T> : ItemsControl<T>, IGrid
{
    [Parameter]
    public int Columns { get; set; } = 1;

    [Parameter]
    public int? ColumnsLg { get; set; }

    [Parameter]
    public int? ColumnsMd { get; set; }

    [Parameter]
    public int? ColumnsSm { get; set; }

    [Parameter]
    public int? ColumnsXs { get; set; }

    [Parameter]
    public double Gap { get; set; }

    [Parameter]
    public double? ColumnGap { get; set; }

    [Parameter]
    public double? RowGap { get; set; }

    protected override void BuildContainerClass(ClassBuilder builder)
    {
        base.BuildContainerClass(builder);
        builder.Add("grid");
    }

    protected override void BuildContainerStyles(StyleBuilder builder)
    {
        base.BuildContainerStyles(builder);
        builder.AddAll(this.GetGridStyles());
    }
}
