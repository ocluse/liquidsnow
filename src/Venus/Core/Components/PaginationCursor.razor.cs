namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that displays pagination controls for cursor-based pagination.
/// </summary>
public partial class PaginationCursor
{

    /// <summary>
    /// Gets or sets the cursor that points to the next page of data.
    /// </summary>
    [Parameter]
    public object? NextCursor { get; set; }

    /// <summary>
    /// Gets or sets the cursor that points to the previous page of data.
    /// </summary>
    [Parameter]
    public object? PreviousCursor { get; set; }

    /// <summary>
    /// Gets or sets a callback that is called when the cursor changes.
    /// </summary>
    [Parameter]
    public EventCallback<object> CursorChanged { get; set; }

    /// <summary>
    /// Gets or sets a function that generates a link for a given cursor.
    /// </summary>
    [Parameter]
    public Func<object?, string>? LinkGenerator { get; set; }

    ///<inheritdoc/>
    public override bool CanGoNext => NextCursor != null;

    ///<inheritdoc/>
    public override bool CanGoPrevious => PreviousCursor != null;

    private async Task MoveNext()
    {
        if (NextCursor != null)
        {
            await ChangeCursor(NextCursor);
        }
    }

    private async Task MovePrevious()
    {
        if (PreviousCursor != null)
        {
            await ChangeCursor(PreviousCursor);
        }
    }

    private async Task ChangeCursor(object cursor)
    {
        await CursorChanged.InvokeAsync(cursor);
    }

    private Dictionary<string, object> GetAttributes(object? input)
    {
        var attributes = new Dictionary<string, object>();
        if (input != null && LinkGenerator != null)
        {
            attributes.Add("href", LinkGenerator.Invoke(input));
        }
        return attributes;
    }
}