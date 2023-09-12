using Microsoft.AspNetCore.Razor.TagHelpers;
using Ocluse.LiquidSnow.Venus.Services;

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
            AddClassAndSetStyle(output);
        }

        protected override void BuildClass(List<string> classList)
        {
            base.BuildClass(classList);
            classList.Add("reading-container");
        }
    }
}
