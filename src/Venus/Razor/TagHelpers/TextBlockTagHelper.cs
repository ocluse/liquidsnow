using Microsoft.AspNetCore.Razor.TagHelpers;
using Ocluse.LiquidSnow.Venus.Services;

namespace Ocluse.LiquidSnow.Venus.Razor.TagHelpers
{
    public class TextBlockTagHelper : TagHelperControlBase
    {
        public int Hierarchy { get; set; } = TextHierarchy.Span;
        public int TextStyle { get; set; } = Values.TextStyle.Body;

        public TextBlockTagHelper(IVenusResolver resolver) : base(resolver)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            //Set the tag name
            output.TagName = _resolver.ResolveTextHierarchy(Hierarchy);

            AddClassAndSetStyle(output);
        }

        protected override void BuildClass(List<string> classList)
        {
            base.BuildClass(classList);
            classList.Add(_resolver.ResolveTextStyle(TextStyle));
        }
    }
}
