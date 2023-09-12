using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class ChipSelect<T> : InputBase<T>
    {
        [Parameter]
        public IEnumerable<T>? Items { get; set; }

        [Parameter]
        public RenderFragment<T>? ItemTemplate { get; set; }

        [Parameter]
        public string? DisplayMemberPath { get; set; }

        [Parameter]
        public Func<T?, string>? DisplayMemberFunc { get; set; }

        protected override T? GetValue(object? value)
        {
            return (T?)value;
        }

        protected override void BuildInputClass(List<string> classList)
        {
            base.BuildInputClass(classList);
            classList.Add("chip-select");
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", GetClass());
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
                        builder.AddContent(7, item.GetDisplayMember(DisplayMemberFunc, DisplayMemberPath));
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
}
