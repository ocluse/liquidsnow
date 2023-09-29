using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class SectionHeading : ControlBase
    {
        [Parameter]
        [EditorRequired]
        public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<TextBlock>(0);
            builder.AddAttribute(1, nameof(TextBlock.Hierarchy), TextHierarchy.H2);
            builder.AddMultipleAttributes(2, GetClassAndStyle());
            builder.AddAttribute(3, nameof(TextBlock.TextStyle), TextStyle.Subtitle);
            builder.AddAttribute(4, nameof(TextBlock.ChildContent), ChildContent);
            builder.CloseComponent();
        }

        protected override void BuildClass(ClassBuilder classBuilder)
        {
            base.BuildClass(classBuilder);
            classBuilder.Add("heading");
        }
    }
}
