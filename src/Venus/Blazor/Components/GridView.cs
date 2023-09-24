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

    protected override void BuildClass(List<string> classList)
    {
        base.BuildClass(classList);
        classList.Add("grid");
    }

    protected override void BuildStyle(List<string> styleList)
    {
        base.BuildStyle(styleList);
        styleList.AddRange(this.GetGridStyles());
    }
}
