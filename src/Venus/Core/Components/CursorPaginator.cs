using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that renders controls for cursor-based pagination.
/// </summary>
public class CursorPaginator<TCursor> : PaginatorBase
{
    /// <summary>
    /// Gets or sets the cursor that points to the next page of data.
    /// </summary>
    [Parameter]
    public TCursor? NextCursor { get; set; }

    /// <summary>
    /// Gets or sets the cursor that points to the previous page of data.
    /// </summary>
    [Parameter]
    public TCursor? PreviousCursor { get; set; }

    /// <summary>
    /// Gets or sets the current cursor.
    /// </summary>
    public TCursor? Cursor { get; set; }

    /// <summary>
    /// Gets or sets a callback that is called when the cursor changes.
    /// </summary>
    [Parameter]
    public EventCallback<TCursor> CursorChanged { get; set; }

    /// <summary>
    /// Gets or sets a function that generates a link for a given cursor.
    /// </summary>
    [Parameter]
    public Func<TCursor?, string>? LinkGenerator { get; set; }

    ///<inheritdoc/>
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        bool reloadDataView = false;

        if(parameters.TryGetValue<TCursor>(nameof(Cursor), out var cursor))
        {
            if (!EqualityComparer<TCursor>.Default.Equals(cursor, Cursor))
            {
                Cursor = cursor;

                reloadDataView = true;
            }
        }

        await base.SetParametersAsync(parameters);

        if (reloadDataView)
        {
            await ReloadDataViewAsync();
        }
    }

    private void BuildPaginatorButton(RenderTreeBuilder builder, bool isNext)
    {
        Dictionary<string, object> attributes = GetPaginatorButtonAttributes(isNext);

        string elementName = LinkGenerator == null ? "button" : "a";

        builder.OpenElement(1, elementName);
        {
            builder.AddMultipleAttributes(2, attributes);
            
            builder.OpenRegion(3);
            {
                BuildSkipButtonContent(builder, isNext);
            }
            builder.CloseRegion();

        }
        builder.CloseElement();
    }

    private Dictionary<string, object> GetPaginatorButtonAttributes(bool isNext)
    {
        var cursor = isNext ? NextCursor : PreviousCursor;

        bool enabled = cursor != null;

        string css = GetItemClass(enabled, false, isNext ? NextClass : PreviousClass);

        Dictionary<string, object> attributes = [];

        attributes.Add("class", css);

        if (LinkGenerator != null && enabled)
        {
            attributes.Add("href", LinkGenerator(cursor));

            attributes.Add("rel", isNext ? "next" : "prev");
        }
        else if (enabled)
        {
            attributes.Add("onclick", EventCallback.Factory.Create(this, async () => await HandleItemClick(cursor)));
        }

        return attributes;
    }

    private async Task HandleItemClick(TCursor? cursor)
    {
        if (!EqualityComparer<TCursor>.Default.Equals(cursor, Cursor))
        {
            await CursorChanged.InvokeAsync(cursor);
        }
    }

    /// <inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "nav");
        {
            builder.AddMultipleAttributes(2, GetAttributes());

            builder.OpenRegion(3);
            {
                BuildPaginatorButton(builder, false);
            }
            builder.CloseRegion();

            builder.OpenRegion(4);
            {
                BuildPaginatorButton(builder, true);
            }
            builder.CloseRegion();
            
        }
        builder.CloseElement();
    }
}
