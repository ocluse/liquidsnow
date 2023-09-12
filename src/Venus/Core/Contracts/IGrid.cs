using System.Text;

namespace Ocluse.LiquidSnow.Venus.Contracts
{
    internal interface IGrid
    {
        int Columns { get; set; }
        int? ColumnsLg { get; set; }
        int? ColumnsMd { get; set; }
        int? ColumnsSm { get; set; }
        int? ColumnsXs { get; set; }
        int Gap { get; set; }
        int? ColumnGap { get; set; }
        int? RowGap { get; set; }
    }
}

