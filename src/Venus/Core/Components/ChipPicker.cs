using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// An input control that allows the user to select a single value from a list of options rendered as chips.
/// </summary>
public class ChipPicker<T> : InputBase<T>, ICollectionView<T>
{
    /// <inheritdoc/>
    [Parameter]
    public IEnumerable<T>? Items { get; set; }

    /// <summary>
    /// Gets or sets an advanced variant for rendering items, which includes a boolean indicating if the item is selected.
    /// </summary>
    [Parameter]
    public RenderFragment<(T, bool)>? AdvancedItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to each item.
    /// </summary>
    [Parameter]
    public string? ItemClass { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public RenderFragment<T>? ItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets the template rendered when there are no items to render.
    /// </summary>
    [Parameter]
    public RenderFragment? EmptyTemplate { get; set; }

    /// <inheritdoc/>
    [Parameter]
    public Func<T?, string>? ToStringFunc { get; set; }

    /// <summary>
    /// Gets or sets the selected values of the control when in multi-select mode.
    /// </summary>
    [Parameter]
    public IReadOnlyCollection<T> SelectedValues { get; set; } = [];

    /// <summary>
    /// Gets or sets the callback for when the selected values of the control change.
    /// </summary>
    [Parameter]
    public EventCallback<IReadOnlyCollection<T>> SelectedValuesChanged { get; set; }

    /// <summary>
    /// Gets or sets the selection mode of the control.
    /// </summary>
    [Parameter]
    public SelectionMode SelectionMode { get; set; }

    ///<inheritdoc/>
    protected override void BuildInputClass(ClassBuilder classBuilder)
    {
        base.BuildInputClass(classBuilder);
        classBuilder.Add(ClassNameProvider.ChipPicker);
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "div");
        {
            builder.AddMultipleAttributes(2, GetAttributes());

            if (Items != null && Items.Any())
            {
                foreach (T item in Items)
                {
                    builder.OpenElement(3, "div");
                    {
                        bool isSelected = SelectionMode == SelectionMode.Multiple ? SelectedValues.Contains(item) : EqualityComparer<T>.Default.Equals(item, Value);
                        builder.AddAttribute(4, "class", ClassBuilder.Join( isSelected 
                            ? ClassNameProvider.ChipPicker_ItemSelected 
                            : ClassNameProvider.ChipPicker_Item, 
                            ItemClass));
                        builder.AddAttribute(5, "onclick", EventCallback.Factory.Create(this, () => HandleOnClick(item)));

                        if (ItemTemplate == null && AdvancedItemTemplate == null)
                        {
                            builder.OpenElement(6, "span");
                            {
                                builder.AddContent(7, item.GetDisplayValue(ToStringFunc));
                            }
                            builder.CloseElement();
                        }
                        else if (AdvancedItemTemplate != null)
                        {
                            builder.AddContent(8, AdvancedItemTemplate, (item, isSelected));
                        }
                        else if (AdvancedItemTemplate != null)
                        {
                            builder.AddContent(9, ItemTemplate, item);
                        }
                    }
                }
            }
            else if (EmptyTemplate != null)
            {
                builder.AddContent(10, EmptyTemplate);
            }
        }
        builder.CloseElement();
    }

    private async Task HandleOnClick(T value)
    {
        await NotifyValueChange(value);

        if (SelectionMode == SelectionMode.Single)
        {
            await SelectedValuesChanged.InvokeAsync([value]);
        }
        else
        {
            List<T> selectedValues = new(SelectedValues);
            if (!selectedValues.Remove(value))
            {
                selectedValues.Add(value);
            }
            await SelectedValuesChanged.InvokeAsync(selectedValues);
        }
    }
}
