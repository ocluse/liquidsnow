namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Represents the state of a data load operation.
/// </summary>
public enum LoadState
{
    /// <summary>
    /// The data is being loaded.
    /// </summary>
    Loading,

    /// <summary>
    /// The data has been loaded or not loaded, but the operation is not in progress.
    /// </summary>
    NotLoading,

    /// <summary>
    /// An error occurred while loading the data.
    /// </summary>
    Error,
}