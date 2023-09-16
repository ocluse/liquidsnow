using Microsoft.AspNetCore.Razor.TagHelpers;
using Ocluse.LiquidSnow.Venus.Services;

namespace Ocluse.LiquidSnow.Venus.Razor.TagHelpers
{
    public class GridTagHelper : TagHelperControlBase, IGrid
    {
        public int Columns { get; set; } = 1;

        public int? ColumnsLg { get; set; }

        public int? ColumnsMd { get; set; }

        public int? ColumnsSm { get; set; }

        public int? ColumnsXs { get; set; }

        public double Gap { get; set; }

        public double? ColumnGap { get; set; }

        public double? RowGap { get; set; }

        public GridTagHelper(IVenusResolver resolver) : base(resolver)
        {

        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            AddClassAndSetStyle(output);
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
