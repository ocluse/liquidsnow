using System.Text;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// The base component for pagination components.
/// </summary>
public abstract class PaginationBase : ControlBase
{
    /// <summary>
    /// Gets a value indicating whether the user can go to the next page.
    /// </summary>
    public abstract bool CanGoNext { get; }

    /// <summary>
    /// Gets a value indicating whether the user can go to the previous page.
    /// </summary>
    public abstract bool CanGoPrevious { get; }


    /// <summary>
    /// Gets or sets the content displayed in the pagination next button.
    /// </summary>
    [Parameter]
    public RenderFragment? PaginationNextContent { get; set; }

    /// <summary>
    /// Gets or sets the content displayed in the pagination previous button.
    /// </summary>
    [Parameter]
    public RenderFragment? PaginationPreviousContent { get; set; }

    /// <summary>
    /// Gets or sets the CSS class that is applied when a pagination navigation item is disabled.
    /// </summary>
    [Parameter]
    public string? DisabledClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to be added to the pagination next button.
    /// </summary>
    [Parameter]
    public string? NextClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to be added to the pagination previous button.
    /// </summary>
    [Parameter]
    public string? PreviousClass { get; set; }

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        classBuilder.Add("pagination");
        base.BuildClass(classBuilder);
    }

    private StringBuilder BaseBuilder(string itemClass, bool disabled)
    {
        StringBuilder sb = new();

        sb.Append("pagination-item");

        if (itemClass != null)
        {
            sb.Append(' ');
            sb.Append(itemClass);
        }

        if (disabled)
        {
            sb.Append(' ');
            if (DisabledClass != null)
            {
                sb.Append(DisabledClass);
            }
            else
            {
                sb.Append("disabled");
            }
        }

        return sb;
    }

    /// <summary>
    /// Returns the CSS class to be applied to the next button.
    /// </summary>
    protected string GetAppliedNextClass()
    {
        StringBuilder sb = BaseBuilder(NextClass ?? "pagination-next", !CanGoNext);
        return sb.ToString();
    }


    /// <summary>
    /// Returns the CSS class to be applied to the previous button.
    /// </summary>
    protected string GetAppliedPreviousClass()
    {
        StringBuilder sb = BaseBuilder(PreviousClass ?? "pagination-previous", !CanGoPrevious);
        return sb.ToString();
    }
}
