using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// An input control that allows the user to select a single value from a list of options.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ChipSelect<T> : InputBase<T>
{
    /// <summary>
    /// The items that will form the options in the select control.
    /// </summary>
    [Parameter]
    public IEnumerable<T>? Items { get; set; }

    /// <summary>
    /// The template for rendering each item in the select control.
    /// </summary>
    [Parameter]
    public RenderFragment<T>? ItemTemplate { get; set; }

    /// <summary>
    /// The path to the property that will be used as the display member.
    /// </summary>
    /// <remarks>
    /// <see cref="DisplayMemberFunc"/> is preferred over this property.
    /// </remarks>
    [Parameter]
    public string? DisplayMemberPath { get; set; }

    /// <summary>
    /// A function that returns the display member for the given item.
    /// </summary>
    [Parameter]
    public Func<T?, string>? DisplayMemberFunc { get; set; }

    ///<inheritdoc/>
    protected override T? GetValue(object? value)
    {
        return (T?)value;
    }

    ///<inheritdoc/>
    protected override void BuildInputClass(ClassBuilder classBuilder)
    {
        base.BuildInputClass(classBuilder);
        classBuilder.Add("chip-select");
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "div");
        builder.AddMultipleAttributes(1, GetAttributes());
        if (Items != null)
        {
            foreach (T item in Items)
            {
                if (item == null)
                {
                    throw new InvalidOperationException("Cannot represent null values");
                }
                builder.OpenElement(2, "div");
                builder.AddAttribute(3, "class", item.Equals(Value) ? "option active" : "option");
                builder.AddAttribute(4, "onclick", () => OnClick(item));
                if (ItemTemplate == null)
                {
                    builder.AddAttribute(5, "style", "padding: 1rem;");
                    builder.OpenElement(6, "div");
                    builder.AddContent(7, item.GetDisplayMemberValue(DisplayMemberFunc, DisplayMemberPath));
                    builder.CloseElement();
                }
                else
                {
                    builder.AddContent(8, ItemTemplate, item);
                }
                builder.CloseComponent();
            }
        }
        builder.CloseElement();
    }

    private async Task OnClick(T value)
    {
        ChangeEventArgs args = new() { Value = value };
        await OnChange(args);
    }
}
