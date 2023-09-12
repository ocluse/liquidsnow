using Microsoft.AspNetCore.Components;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class PaginationBase : ControlBase
    {
        [Parameter]
        public RenderFragment? PaginationNextContent { get; set; }

        [Parameter]
        public RenderFragment? PaginationBackContent { get; set; }

        protected override void BuildClass(List<string> classList)
        {
            classList.Add("pagination");
            base.BuildClass(classList);
        }
    }
}
