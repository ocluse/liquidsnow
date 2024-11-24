using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A component that renders controls for offset-based pagination.
/// </summary>
public class OffsetPaginator : PaginatorBase
{
    /// <summary>
    /// Gets or sets the current page that is being displayed.
    /// </summary>
    [Parameter]
    public int Page { get; set; }

    /// <summary>
    /// Gets or sets the callback that is called when the page changes.
    /// </summary>
    [Parameter]
    public EventCallback<int> PageChanged { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of pagination items that can be displayed.
    /// </summary>
    [Parameter]
    public int? MaxPaginatorItems { get; set; }

    /// <summary>
    /// Gets or sets the number of items that can be displayed in a page.
    /// </summary>
    [Parameter]
    public int? PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of items available used to compute the total number of pages.
    /// </summary>
    [Parameter]
    public int TotalItemCount { get; set; }

    /// <summary>
    /// Gets or sets the template that is used to render page buttons.
    /// </summary>
    [Parameter]
    public RenderFragment<int>? ItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets a function that is used to generate links to pages.
    /// </summary>
    [Parameter]
    public Func<int, string>? LinkGenerator { get; set; }

    private int TotalPages => (int)Math.Ceiling(TotalItemCount / (double)(PageSize ?? Resolver.DefaultPageSize));

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "nav");
        {
            builder.AddMultipleAttributes(2, GetAttributes());

            if (TotalPages > 0)
            {
                int maxPaginatorItems = MaxPaginatorItems ?? Resolver.DefaultMaxPaginatorItems;

                int startPage = Page - maxPaginatorItems / 2;

                if (startPage < 0)
                {
                    startPage = 0;
                }

                int endPage = startPage + maxPaginatorItems - 1;

                if (endPage > TotalPages)
                {
                    endPage = TotalPages - 1;
                    startPage = endPage - maxPaginatorItems + 1;

                    if (startPage < 0)
                    {
                        startPage = 0;
                    }
                }

                BuildSkipButton(builder, false);

                for (int i = startPage; i <= endPage; i++)
                {
                    BuildPaginatorButton(builder, i, null, (builder) =>
                    {
                        if (ItemTemplate != null)
                        {
                            builder.AddContent(0, ItemTemplate(i));
                        }
                        else
                        {
                            builder.AddContent(1, i + 1);
                        }
                    });
                }

                BuildSkipButton(builder, true);
            }
        }
        builder.CloseElement();
    }

    private void BuildPaginatorButton(RenderTreeBuilder builder, int page, string? customCss, Action<RenderTreeBuilder> buildContent)
    {
        Dictionary<string, object> attributes = GetPaginatorButtonAttributes(page, false, customCss);

        if (LinkGenerator != null)
        {
            builder.OpenElement(0, "a");
        }
        else
        {
            builder.OpenElement(1, "button");
        }
        {
            builder.AddMultipleAttributes(1, attributes);
            buildContent(builder);
        }
        builder.CloseElement();
    }

    private Dictionary<string, object> GetPaginatorButtonAttributes(int page, bool skipper, string? cssClass)
    {
        bool enabled = page >= 0 && page < TotalPages;

        string css = GetItemClass(enabled, page == Page && !skipper, cssClass);

        Dictionary<string, object> attributes = [];
        attributes.Add("class", css);

        if (LinkGenerator != null && enabled)
        {
            attributes.Add("href", LinkGenerator(page));

            if (page > 0)
            {
                attributes.Add("rel", "prev");
            }
            else if (page < TotalPages - 1)
            {
                attributes.Add("rel", "next");
            }
        }
        else if (enabled)
        {
            attributes.Add("onclick", EventCallback.Factory.Create(this, async () => await OnClickItem(page)));
        }

        return attributes;
    }

    private async Task OnClickItem(int page)
    {
        if(page != Page)
        {
            var previousPage = Page;

            await PageChanged.InvokeAsync(page);

            if (previousPage != Page)
            {
                await ReloadDataViewAsync();
            }
        }
    }

    private void BuildSkipButton(RenderTreeBuilder builder, bool isNext)
    {
        int page = isNext ? Page + 1 : Page - 1;
        string? cssClass = isNext ? NextClass : PreviousClass;

        BuildPaginatorButton(builder, page, cssClass, (builder) =>
        {
            BuildSkipButtonContent(builder, isNext);
        });
    }
}
