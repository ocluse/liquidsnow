namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// A contract for components that implement a grid layout.
/// </summary>
public interface IGrid
{
    /// <summary>
    /// The default number of columns in the grid.
    /// </summary>
    int Columns { get; set; }

    /// <summary>
    /// The number of columns in the grid when the viewport is large e.g. desktop.
    /// </summary>
    int? ColumnsLg { get; set; }

    /// <summary>
    /// The number of columns in the grid when the viewport is medium sized e.g. laptop.
    /// </summary>
    int? ColumnsMd { get; set; }

    /// <summary>
    /// The number of columns in the grid when the viewport is small e.g. tablet.
    /// </summary>
    int? ColumnsSm { get; set; }

    /// <summary>
    /// The number of columns in the grid when the viewport is extra small e.g. mobile.
    /// </summary>
    int? ColumnsXs { get; set; }

    /// <summary>
    /// The default gap between the columns and rows in the grid.
    /// </summary>
    double Gap { get; set; }

    /// <summary>
    /// Gets or sets the unit of measurement for the gap.
    /// </summary>
    CssUnit? GapUnit { get; set; }

    /// <summary>
    /// The gap between the columns and rows in the grid when the viewport is large e.g. desktop.
    /// </summary>
    double? GapLg { get; set; }

    /// <summary>
    /// The gap between the columns and rows in the grid when the viewport is medium sized e.g. laptop.
    /// </summary>
    double? GapMd { get; set; }

    /// <summary>
    /// The gap between the columns and rows in the grid when the viewport is small e.g. tablet.
    /// </summary>
    double? GapSm { get; set; }

    /// <summary>
    /// The gap between the columns and rows in the grid when the viewport is extra small e.g. mobile.
    /// </summary>
    double? GapXs { get; set; }

    /// <summary>
    /// The gap between the columns in the grid.
    /// </summary>
    double? ColumnGap { get; set; }

    /// <summary>
    /// The gap between the rows in the grid.
    /// </summary>
    double? RowGap { get; set; }
}

