using Microsoft.AspNetCore.Components.Rendering;
using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Utils;
using Ocluse.LiquidSnow.Venus.Contracts.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// An input that renders a dropdown.
/// </summary>
public class Dropdown<TValue> : FieldBase<TValue>, ICollectionView<TValue>, IAuxiliaryContentFieldComponent
{
    private readonly string _anchorName = "--" + IdGenerator.GenerateId(IdKind.Standard, 6).ToLowerInvariant();
    
    private ElementReference _popoverElement;

    ///<inheritdoc/>
    [Parameter]
    public IEnumerable<TValue>? Items { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public RenderFragment<TValue>? ItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets the template for rendering items in the dropdown, which includes a boolean indicating if the item is selected.
    /// </summary>
    /// <remarks>
    /// The bool only has value when being rendered from the list. When being rendered from the input, it will always be null.
    /// </remarks>
    [Parameter]
    public RenderFragment<(TValue Value, bool? Selected)>? AdvancedItemTemplate { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the dropdown when it is open.
    /// </summary>
    [Parameter]
    public string? OpenClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class applied to the dropdown when it is closed.
    /// </summary>
    [Parameter]
    public string? ClosedClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to apply to each item in the dropdown.
    /// </summary>
    [Parameter]
    public string? ItemClass { get; set; }

    /// <summary>
    /// Gets or sets the CSS class to apply to the dropdown's popover.
    /// </summary>
    [Parameter]
    public string? PopoverClass { get; set; }

    /// <summary>
    /// Gets or sets the content that is used as the input's placeholder.
    /// </summary>
    [Parameter]
    public RenderFragment? PlaceholderContent { get; set; }

    ///<inheritdoc/>
    [Parameter]
    public Func<TValue?, string>? ToStringFunc { get; set; }

    [Inject]
    private IVenusJSInterop JSInterop { get; set; } = default!;

    ///<inheritdoc/>
    protected override void BuildInputClass(ClassBuilder classBuilder)
    {
        base.BuildInputClass(classBuilder);
        classBuilder.Add(ClassNameProvider.Dropdown);
    }

    ///<inheritdoc/>
    protected override void BuildInput(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "div");
        {
            builder.AddAttribute(2, "class", ClassBuilder.Join(ClassNameProvider.Field_Input, InputClass));
            builder.AddAttribute(3, "onclick", EventCallback.Factory.Create(this, HandleDropdownClick));
            if (Value != null)
            {
                if (AdvancedItemTemplate != null)
                {
                    builder.AddContent(4, AdvancedItemTemplate((Value, null)));
                }
                else if (ItemTemplate != null)
                {
                    builder.AddContent(5, ItemTemplate(Value));
                }
                else
                {
                    builder.OpenElement(6, "span");
                    {
                        builder.AddContent(7, Value.GetDisplayValue(ToStringFunc));
                    }
                    builder.CloseElement();
                }
            }
            else
            {
                if (PlaceholderContent != null)
                {
                    builder.AddContent(8, PlaceholderContent);
                }
                else if (Placeholder.IsNotEmpty())
                {
                    builder.OpenElement(9, "span");
                    {
                        builder.AddAttribute(10, "class", ClassNameProvider.Dropdown_Placeholder);
                        builder.AddContent(11, Placeholder);
                    }
                    builder.CloseElement();
                }
            }
        }
        builder.CloseElement();
    }

    ///<inheritdoc/>
    public void BuildAuxiliaryContent(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "div");
        {
            builder.AddAttribute(2, "class", ClassBuilder.Join(ClassNameProvider.Dropdown_Popover, PopoverClass));
            builder.AddAttribute(3, "style", $"position-anchor: {_anchorName};");
            builder.AddAttribute(4, "popover");
            builder.AddElementReferenceCapture(5, (value) => _popoverElement = value);
            if (Items != null && Items.Any())
            {
                foreach (TValue item in Items)
                {
                    builder.OpenElement(6, "div");
                    {
                        builder.SetKey(item);

                        bool selected = EqualityComparer<TValue>.Default.Equals(item, Value);
                        builder.AddAttribute(7, "class", ClassBuilder.Join(ClassNameProvider.Dropdown_Item, selected
                            ? ClassNameProvider.Dropdown_ItemSelected
                            : null,
                            ItemClass));
                        builder.AddAttribute(8, "onclick", EventCallback.Factory.Create(this, async () => await HandleItemClickAsync(item)));

                        if (AdvancedItemTemplate != null)
                        {
                            builder.AddContent(9, AdvancedItemTemplate, (item, selected));
                        }
                        else if (ItemTemplate != null)
                        {
                            builder.AddContent(10, ItemTemplate, item);
                        }
                        else
                        {
                            builder.OpenElement(12, "span");
                            {
                                builder.AddContent(13, item.GetDisplayValue(ToStringFunc));
                            }
                            builder.CloseElement();
                        }
                    }
                    builder.CloseElement();
                }
            }
            builder.CloseElement();
        }
    }

    ///<inheritdoc/>
    protected override void BuildAttributes(IDictionary<string, object> attributes)
    {
        base.BuildAttributes(attributes);
        attributes.Add("data-anchor-name", _anchorName);
    }

    ///<inheritdoc/>
    protected override void BuildStyle(StyleBuilder styleBuilder)
    {
        base.BuildStyle(styleBuilder);
        styleBuilder.Add("anchor-name", _anchorName);
    }

    private async Task HandleDropdownClick()
    {
        await JSInterop.ShowPopoverAsync(_popoverElement);
    }

    private async Task HandleItemClickAsync(TValue item)
    {
        if (EqualityComparer<TValue>.Default.Equals(item, Value))
        {
            await NotifyValueChange(default);
        }
        else
        {
            await NotifyValueChange(item);
        }

        await JSInterop.HidePopoverAsync(_popoverElement);
    }
}
