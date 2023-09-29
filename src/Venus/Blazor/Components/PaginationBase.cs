namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class PaginationBase : ControlBase
    {
        [Parameter]
        public RenderFragment? PaginationNextContent { get; set; }

        [Parameter]
        public RenderFragment? PaginationBackContent { get; set; }

        protected override void BuildClass(ClassBuilder classBuilder)
        {
            classBuilder.Add("pagination");
            base.BuildClass(classBuilder);
        }
    }
}
