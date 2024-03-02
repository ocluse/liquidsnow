using System.Text;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public abstract class PaginationBase : ControlBase
    {
        public abstract bool CanGoNext { get; }

        public abstract bool CanGoPrevious { get; }

        [Parameter]
        public RenderFragment? PaginationNextContent { get; set; }

        [Parameter]
        public RenderFragment? PaginationPreviousContent { get; set; }

        [Parameter]
        public string? DisabledClass { get; set; }

        [Parameter]
        public string? NextClass { get; set; }

        [Parameter]
        public string? PreviousClass { get; set; }

        protected override void BuildClass(ClassBuilder classBuilder)
        {
            classBuilder.Add("pagination");
            base.BuildClass(classBuilder);
        }

        private StringBuilder BaseBuilder(string itemClass, bool disabled)
        {
            StringBuilder sb = new();

            sb.Append("pagination-item");

            if (itemClass != null)
            {
                sb.Append(' ');
                sb.Append(itemClass);
            }

            if (disabled)
            {
                sb.Append(' ');
                if (DisabledClass != null)
                {
                    sb.Append(DisabledClass);
                }
                else
                {
                    sb.Append("disabled");
                }
            }

            return sb;
        }

        protected string GetAppliedNextClass()
        {
            StringBuilder sb = BaseBuilder(NextClass ?? "pagination-next", !CanGoNext);
            return sb.ToString();
        }

        protected string GetAppliedPreviousClass()
        {
            StringBuilder sb = BaseBuilder(PreviousClass ?? "pagination-previous", !CanGoPrevious);
            return sb.ToString();
        }
    }
}
