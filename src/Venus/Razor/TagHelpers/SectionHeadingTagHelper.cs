using Microsoft.AspNetCore.Razor.TagHelpers;
using Ocluse.LiquidSnow.Venus.Services;

namespace Ocluse.LiquidSnow.Venus.Razor.TagHelpers
{
    public class SectionHeadingTagHelper : TagHelperControlBase
    {
        public SectionHeadingTagHelper(IVenusResolver resolver) : base(resolver)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "h2";
            AddClassAndSetStyle(output);
        }

        protected override void BuildClass(List<string> classList)
        {
            base.BuildClass(classList);
            classList.Add(_resolver.ResolveTextStyle(TextStyle.Subtitle));
            classList.Add("heading");
        }
    }

    [HtmlTargetElement("avatar", TagStructure = TagStructure.WithoutEndTag)]
    public class AvatarTagHelper : TagHelperControlBase
    {
        public AvatarTagHelper(IVenusResolver resolver) : base(resolver)
        {
        }

        public string Src { get; set; } = string.Empty;

        public string? UserId { get; set; }

        public int Size { get; set; } = DefaultSize.Size48;

        public string? SrcOnError { get; set; } = "/images/anonymous.svg";

        protected override void BuildClass(List<string> classList)
        {
            base.BuildClass(classList);
            classList.Add("avatar");
        }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";

            string src = string.IsNullOrEmpty(UserId) ? Src : _resolver.ResolveAvatarId(UserId);

            AddClassAndSetStyle(output);

            output.Attributes.Add("src", src);
            output.Attributes.Add("height", Size);
            output.Attributes.Add("width", Size);
            output.Attributes.Add("onerror", $"this.src ='{SrcOnError}';this.onerror=''");
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
