using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Blazor.Components
{
    public class Avatar : ControlBase
    {
        [Parameter]
        public string Src { get; set; } = string.Empty;

        [Parameter]
        public string? UserId { get; set; }

        [Parameter]
        public int? Size { get; set; }

        [Parameter]
        public string? SrcOnError { get; set; } = "/images/anonymous.svg";

        protected override void BuildClass(ClassBuilder classBuilder)
        {
            base.BuildClass(classBuilder);
            classBuilder.Add("avatar");
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "img");

            string src = string.IsNullOrEmpty(UserId) ? Src : VenusResolver.ResolveAvatarId(UserId);

            builder.AddAttribute(1, "src", src);
            builder.AddAttribute(2, "height", Size ?? Resolver.DefaultAvatarSize);
            builder.AddAttribute(2, "width", Size ?? Resolver.DefaultAvatarSize);
            builder.AddAttribute(3, "onerror", $"this.src ='{SrcOnError}';this.onerror=''");
            builder.AddMultipleAttributes(4, GetClassAndStyle());
            builder.CloseElement();
        }
    }
}
