using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class Grid : ControlBase, IGrid
    {
        [Parameter]
        public int Columns { get; set; } = 1;

        [Parameter]
        public int? ColumnsLg { get; set; }

        [Parameter]
        public int? ColumnsMd { get; set; }

        [Parameter]
        public int? ColumnsSm { get; set; }

        [Parameter]
        public int? ColumnsXs { get; set; }

        [Parameter]
        public int Gap { get; set; }

        [Parameter]
        public int? ColumnGap { get; set; }

        [Parameter]
        public int? RowGap { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");

            Dictionary<string, object> attributes = new()
            {
                { "class", GetClass() },
                {"style", GetStyle() },
            };

            builder.AddMultipleAttributes(1, attributes);

            BuildContent(builder);

            builder.CloseElement();
        }

        protected virtual void BuildContent(RenderTreeBuilder builder)
        {
            if (ChildContent != null)
            {
                builder.AddContent(2, ChildContent);
            }
        }

        protected override void BuildClass(List<string> classList)
        {
            base.BuildClass(classList);
            classList.Add("grid");
        }

        protected override void BuildStyle(List<string> styleList)
        {
            base.BuildStyle(styleList);
            styleList.AddRange(this.GetGridStyles());
        }
    }
}
