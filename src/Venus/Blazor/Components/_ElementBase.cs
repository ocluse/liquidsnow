using Microsoft.AspNetCore.Components;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public abstract class ElementBase : ComponentBase
    {
        [Parameter]
        public string? Padding { get; set; }

        [Parameter]
        public string? Margin { get; set; }

        [Parameter]
        public string? Class { get; set; }

        protected virtual void BuildStyle(List<string> styleList)
        {
        }

        protected virtual void BuildClass(List<string> classList)
        {
        }

        protected string GetStyle()
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

            if (styleList.Count > 0)
            {
                return string.Join(';', styleList);
            }
            else
            {
                return string.Empty;
            }
        }

        protected string GetClass()
        {
            List<string> classList = new();
            BuildClass(classList);

            if (!string.IsNullOrEmpty(Class))
            {
                classList.Add(Class);
            }

            if (classList.Count > 0)
            {
                return string.Join(" ", classList);

            }
            else
            {
                return string.Empty;
            }
        }
    }
}