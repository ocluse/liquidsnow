namespace Ocluse.LiquidSnow.Http;

/// <summary>
/// The type of query to perform.
/// </summary>
public enum QueryType
{
    /// <summary>
    /// The query will be paged using a cursor.
    /// </summary>
    Cursor,

    /// <summary>
    /// The query will be filtered using the specified ids.
    /// </summary>
    Ids,

    /// <summary>
    /// The query will be paged using a page offset.
    /// </summary>
    Offset,

    /// <summary>
    /// The query will be filtered using a custom method.
    /// </summary>
    Custom
}