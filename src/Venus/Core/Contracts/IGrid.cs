namespace Ocluse.LiquidSnow.Venus.Contracts
{
    internal interface IGrid
    {
        int Columns { get; set; }
        int? ColumnsLg { get; set; }
        int? ColumnsMd { get; set; }
        int? ColumnsSm { get; set; }
        int? ColumnsXs { get; set; }
        double Gap { get; set; }
        double? ColumnGap { get; set; }
        double? RowGap { get; set; }
    }
}

