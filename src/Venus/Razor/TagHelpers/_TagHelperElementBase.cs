using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ocluse.LiquidSnow.Venus.Razor.TagHelpers
{
    public abstract class TagHelperElementBase : TagHelper
    {
        public string? Padding { get; set; }

        public string? Margin { get; set; }

        public string? Class { get; set; }

        protected virtual void BuildStyle(StyleBuilder styleBuilder) { }

        protected virtual void BuildClass(ClassBuilder classBuilder) { }

        protected string GetStyle()
        {
            StyleBuilder styleBuilder = new();

            if (!string.IsNullOrEmpty(Margin))
            {
                styleBuilder.Add($"margin: {Margin.ParseThicknessValues()};");
            }

            if (!string.IsNullOrEmpty(Padding))
            {
                styleBuilder.Add($"padding: {Padding.ParseThicknessValues()};");
            }

            BuildStyle(styleBuilder);

            return styleBuilder.Build();
        }

        protected string GetClass()
        {
            ClassBuilder classBuilder = new();
            
            BuildClass(classBuilder);

            classBuilder.Add(Class);

            return classBuilder.Build();
        }

        protected void SetClassAndSetStyle(TagHelperOutput output)
        {
            output.Attributes.SetAttribute("style", GetStyle());
            output.Attributes.SetAttribute("class", GetClass());
        }
    }
}