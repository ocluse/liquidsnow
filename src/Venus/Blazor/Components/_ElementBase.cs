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

        [Parameter]
        public string? Style { get; set; }

        protected virtual void BuildStyle(StyleBuilder builder) { }

        protected virtual void BuildClass(ClassBuilder builder) { }

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

            styleBuilder.Add(Style);

            return styleBuilder.Build();
        }

        protected string GetClass()
        {
            ClassBuilder classBuilder = new();

            BuildClass(classBuilder);

            classBuilder.Add(Class);

            return classBuilder.Build();

        }

        protected Dictionary<string, object> GetClassAndStyle()
        {
            Dictionary<string, object> attributes = new()
            {
                { "class", GetClass() },
                {"style", GetStyle() },
            };
            return attributes;
        }
    }
}