namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Represents the type of load operation to perform.
/// </summary>
public enum LoadType
{
    /// <summary>
    /// Load the data afresh, for example when loading the data for the first time or when the data is invalidated.
    /// </summary>
    Refresh,

    /// <summary>
    /// Load the next page of data, for example when the user scrolls down or clicks a "Load more" button.
    /// </summary>
    Append,

    /// <summary>
    /// Load the previous page of data, for example when the user scrolls up or clicks a "Load previous" button.
    /// </summary>
    Prepend
}
