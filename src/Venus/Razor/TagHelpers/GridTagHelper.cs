using Microsoft.AspNetCore.Razor.TagHelpers;

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
            SetClassAndSetStyle(output);
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
