using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ocluse.LiquidSnow.Venus.Razor.TagHelpers
{
    public class ReadingContainerTagHelper : TagHelperControlBase
    {
        public ReadingContainerTagHelper(IVenusResolver resolver) : base(resolver)
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
            classBuilder.Add("reading-container");
        }
    }
}
