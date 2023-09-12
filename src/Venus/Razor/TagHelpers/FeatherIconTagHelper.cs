using Microsoft.AspNetCore.Razor.TagHelpers;

using Ocluse.LiquidSnow.Venus.Services;

namespace Ocluse.LiquidSnow.Venus.Razor.TagHelpers
{
    [HtmlTargetElement("feather-icon", TagStructure = TagStructure.WithoutEndTag)]
    public class FeatherIconTagHelper : TagHelperControlBase
    {
        public FeatherIconTagHelper(IVenusResolver resolver) : base(resolver)
        {
        }

        public required string Icon { get; set; }
        public int Size { get; set; } = DefaultSize.Size24;
        public int StrokeWidth { get; set; } = 2;

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "svg";

            AddClassAndSetStyle(output);

            output.Attributes.Add("height", Size);
            output.Attributes.Add("width", Size);
            output.Attributes.Add("xmlns", "http://www.w3.org/2000/svg");
            output.Attributes.Add("viewBox", "0 0 24 24");
            output.Attributes.Add("fill", "none");
            output.Attributes.Add("stroke", "currentColor");
            output.Attributes.Add("stroke-width", StrokeWidth);
            output.Attributes.Add("stroke-linecap", "round");
            output.Attributes.Add("stroke-linejoin", "round");

            output.Content.SetHtmlContent(Icon);
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
