using System.Text;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component for displaying pagination controls for offset-based pagination.
/// </summary>
public partial class PaginationOffset
{
    private int _totalPages;

    /// <summary>
    /// Gets or sets the current page that is being displayed.
    /// </summary>
    [Parameter]
    public int CurrentPage { get; set; }

    /// <summary>
    /// Gets or sets the number of items that can be displayed in a page.
    /// </summary>
    [Parameter]
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of items available.
    /// </summary>
    [Parameter]
    public int TotalItems { get; set; }

    /// <summary>
    /// Gets or sets the callback that is called when the page changes.
    /// </summary>
    [Parameter]
    public EventCallback<int> PageChanged { get; set; }

    /// <summary>
    /// Gets or sets the template that is used to render page buttons.
    /// </summary>
    [Parameter]
    public RenderFragment<int>? PaginationItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets the CSS class that is applied to the active pagination item.
    /// </summary>
    [Parameter]
    public string? ActiveItemClass { get; set; }

    /// <summary>
    /// Gets or sets a function that is used to generate links to pagination pages attached to each pagination item.
    /// </summary>
    [Parameter]
    public Func<int, string>? LinkGenerator { get; set; }

    ///<inheritdoc/>
    public override bool CanGoNext => CurrentPage < _totalPages - 1;

    ///<inheritdoc/>
    public override bool CanGoPrevious => CurrentPage > 0;

    private Dictionary<string, object> GetItemAttributes(int buttonNumber, bool isActive)
    {
        var attributes = new Dictionary<string, object>();
        if (LinkGenerator != null)
        {
            if (!isActive)
            {
                attributes.Add("href", LinkGenerator(buttonNumber));
            }
        }
        return attributes;
    }

    private Dictionary<string, object> GetNextAttributes()
    {
        var attributes = new Dictionary<string, object>();
        if (LinkGenerator != null)
        {
            if (CurrentPage < _totalPages - 1)
            {
                attributes.Add("href", LinkGenerator(CurrentPage + 1));
            }
        }
        return attributes;
    }

    private Dictionary<string, object> GetPreviousAttributes()
    {
        var attributes = new Dictionary<string, object>();
        if (LinkGenerator != null)
        {
            if (CurrentPage > 0)
            {
                attributes.Add("href", LinkGenerator(CurrentPage - 1));
            }
        }
        return attributes;
    }

    private string GetPaginationItemClass(bool isActive)
    {
        StringBuilder sb = new();
        sb.Append("pagination-item");
        if (isActive)
        {
            sb.Append(' ');
            sb.Append(ActiveItemClass ?? "active");
        }

        return sb.ToString();
    }

    ///<inheritdoc/>
    protected override void OnParametersSet()
    {
        _totalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
        base.OnParametersSet();
    }

    private async Task ChangePage(int page)
    {
        CurrentPage = page;
        await PageChanged.InvokeAsync(page);
    }

    private async Task MoveNext()
    {
        CurrentPage++;
        await PageChanged.InvokeAsync(CurrentPage);
    }

    private async Task MovePrevious()
    {
        CurrentPage--;
        await PageChanged.InvokeAsync(CurrentPage);
    }
}