using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class TableView<T> : ItemsControl<T>
    {
        protected override string ItemElement { get; } = "tr";

        protected override string ContainerElement { get; } = "table";

        [Parameter]
        public RenderFragment? HeaderTemplate { get; set; }

        [Parameter]
        public RenderFragment? FooterTemplate { get; set; }

        protected override void BuildContainerClass(ClassBuilder builder)
        {
            base.BuildContainerClass(builder);
            builder.Add("table");
        }

        protected override void BuildFound(RenderTreeBuilder builder)
        {
            if (HeaderTemplate != null)
            {
                builder.OpenElement(30, "thead");
                builder.AddContent(31, HeaderTemplate);
                builder.CloseElement();
            }

            builder.OpenElement(32, "tbody");
            base.BuildFound(builder);
            builder.CloseElement();

            if (FooterTemplate != null)
            {
                builder.OpenElement(33, "tfoot");
                builder.AddContent(34, FooterTemplate);
                builder.CloseElement();
            }
        }
    }
}
