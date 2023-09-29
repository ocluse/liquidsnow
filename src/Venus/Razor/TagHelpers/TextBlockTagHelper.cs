using Microsoft.AspNetCore.Razor.TagHelpers;

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

            SetClassAndSetStyle(output);
        }

        protected override void BuildClass(ClassBuilder classBuilder)
        {
            base.BuildClass(classBuilder);
            classBuilder.Add(_resolver.ResolveTextStyle(TextStyle));
        }
    }
}
