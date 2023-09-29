using Microsoft.AspNetCore.Razor.TagHelpers;

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
            SetClassAndSetStyle(output);
        }

        protected override void BuildClass(ClassBuilder classBuilder)
        {
            base.BuildClass(classBuilder);
            classBuilder.Add(_resolver.ResolveTextStyle(TextStyle.Subtitle));
            classBuilder.Add("heading");
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

        protected override void BuildClass(ClassBuilder classBuilder)
        {
            base.BuildClass(classBuilder);
            classBuilder.Add("avatar");
        }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "img";

            string src = string.IsNullOrEmpty(UserId) ? Src : _resolver.ResolveAvatarId(UserId);

            SetClassAndSetStyle(output);

            output.Attributes.Add("src", src);
            output.Attributes.Add("height", Size);
            output.Attributes.Add("width", Size);
            output.Attributes.Add("onerror", $"this.src ='{SrcOnError}';this.onerror=''");
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
