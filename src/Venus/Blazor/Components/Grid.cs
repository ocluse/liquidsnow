using Microsoft.AspNetCore.Components.Rendering;

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
        public double Gap { get; set; }

        [Parameter]
        public double? ColumnGap { get; set; }

        [Parameter]
        public double? RowGap { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");

            builder.AddMultipleAttributes(1, GetClassAndStyle());

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

        protected override void BuildClass(ClassBuilder classBuilder)
        {
            base.BuildClass(classBuilder);
            classBuilder.Add("grid");
        }

        protected override void BuildStyle(StyleBuilder styleBuilder)
        {
            base.BuildStyle(styleBuilder);
            styleBuilder.AddAll(this.GetGridStyles());
        }
    }
}
