using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace Ocluse.LiquidSnow.Venus.Razor.TagHelpers
{
    public abstract class TagHelperElementBase : TagHelper
    {
        public string? Padding { get; set; }

        public string? Margin { get; set; }

        public string? Class { get; set; }

        protected virtual void BuildStyle(List<string> styleList)
        {
        }

        protected virtual void BuildClass(List<string> classList)
        {
        }

        protected IEnumerable<string> GetStyle()
        {

            List<string> styleList = new();

            if (!string.IsNullOrEmpty(Margin))
            {
                styleList.Add($"margin: {Margin.ParseThicknessValues()};");
            }

            if (!string.IsNullOrEmpty(Padding))
            {
                styleList.Add($"padding: {Padding.ParseThicknessValues()};");
            }

            BuildStyle(styleList);

            return styleList;
        }

        protected IEnumerable<string> GetClass()
        {
            List<string> classList = new();
            BuildClass(classList);

            if (!string.IsNullOrEmpty(Class))
            {
                classList.AddRange(Class.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            }


            return classList;
        }

        protected void AddClassAndSetStyle(TagHelperOutput output)
        {
            foreach (var cls in GetClass())
            {
                output.AddClass(cls, HtmlEncoder.Default);
            }

            var styles = GetStyle();
            if (styles.Any())
            {
                output.Attributes.SetAttribute("style", string.Join(';', styles));
            }
        }
    }
}