using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;
using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Utils;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// An input that renders a dropdown.
/// </summary>
public class Dropdown<TValue> : FieldBase<TValue>, ICollectionView<TValue>, IDropdown, IAsyncDisposable
{
    private bool _open;
    private string _dropdownId = IdGenerator.GenerateId(IdKind.Standard, 6);
    private DotNetObjectReference<IDropdown> _dotNetObj = default!;
    
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
    /// Gets or sets the CSS class to apply to the dropdown list.
    /// </summary>
    [Parameter]
    public string? ListClass { get; set; }

    /// <summary>
    /// Gets or sets the content that is used as the input's placeholder.
    /// </summary>
    [Parameter]
    public RenderFragment? PlaceholderContent { get; set; }

    [Inject]
    private IVenusJSInterop JSInterop { get; set; } = default!;

    ///<inheritdoc/>
    protected override bool HasAuxiliaryContent => true;

    ///<inheritdoc/>
    public Func<TValue?, string>? ToStringFunc { get; set; }


    ///<inheritdoc/>
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _dotNetObj = DotNetObjectReference.Create((IDropdown)this);
        await JSInterop.InitializeDropdownWatcher();
    }

    ///<inheritdoc/>
    protected override void BuildInputClass(ClassBuilder classBuilder)
    {
        base.BuildInputClass(classBuilder);
        classBuilder.Add(ClassNameProvider.Dropdown)
            .AddIf(_open, ClassNameProvider.DropdownOpen, OpenClass)
            .AddIf(!_open, ClassNameProvider.DropdownClosed, ClosedClass);
    }

    ///<inheritdoc/>
    protected override void BuildInput(RenderTreeBuilder builder)
    {
        if (Value != null)
        {
            if (AdvancedItemTemplate != null)
            {
                builder.AddContent(1, AdvancedItemTemplate((Value, null)));
            }
            else if (ItemTemplate != null)
            {
                builder.AddContent(2, ItemTemplate(Value));
            }
            else
            {
                builder.OpenElement(3, "span");
                {
                    builder.AddContent(4, Value.GetDisplayValue(ToStringFunc));
                }
                builder.CloseElement();
            }
        }
        else
        {
            if (PlaceholderContent != null)
            {
                builder.AddContent(5, PlaceholderContent);
            }
            else if (Placeholder.IsNotEmpty())
            {
                builder.OpenElement(6, "span");
                {
                    builder.AddContent(7, Placeholder);
                }
                builder.CloseElement();
            }
        }
    }

    ///<inheritdoc/>
    protected override void BuildAuxiliaryContent(RenderTreeBuilder builder)
    {
        builder.OpenElement(1, "div");
        {
            builder.AddAttribute(2, "class", ClassBuilder.Join(ClassNameProvider.Dropdown_List, ListClass));

            if (Items != null && Items.Any())
            {
                foreach (TValue item in Items)
                {
                    builder.OpenElement(3, "div");
                    {
                        builder.SetKey(item);

                        bool selected = EqualityComparer<TValue>.Default.Equals(item, Value);
                        builder.AddAttribute(4, "class", ClassBuilder.Join(selected
                            ? ClassNameProvider.Dropdown_ItemSelected
                            : ClassNameProvider.Dropdown_Item,
                            ItemClass));
                        builder.AddAttribute(5, "onclick", EventCallback.Factory.Create(this, async () => await HandleItemClickAsync(item)));

                        if (AdvancedItemTemplate != null)
                        {
                            builder.AddContent(6, AdvancedItemTemplate, (item, selected));
                        }
                        else if (ItemTemplate != null)
                        {
                            builder.AddContent(7, ItemTemplate, item);
                        }
                        else
                        {
                            builder.OpenElement(8, "span");
                            {
                                builder.AddContent(10, item.GetDisplayValue(ToStringFunc));
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
        attributes.Add("onclick", EventCallback.Factory.Create(this, HandleDropdownClick));
        attributes.Add("data-dropdown-id", _dropdownId);
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
    }

    private async Task HandleDropdownClick()
    {
        if (_open)
        {
            await JSInterop.UnwatchDropdownAsync(_dotNetObj);
        }
        else
        {
            await JSInterop.WatchDropdownAsync(_dotNetObj, _dropdownId);
        }

        _open = !_open;

        await InvokeAsync(StateHasChanged);
    }

    ///<inheritdoc/>
    [JSInvokable]
    public async Task CloseDropdown()
    {
        _open = false;
        await InvokeAsync(StateHasChanged);
    }

    ///<inheritdoc cref="DisposeAsync"/>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        await JSInterop.UnwatchDropdownAsync(_dotNetObj);
    }

    ///<inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();

        Dispose(false);

        GC.SuppressFinalize(this);
    }
}
