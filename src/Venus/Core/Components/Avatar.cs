using Microsoft.AspNetCore.Components.Rendering;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A control for showing avatar images.
/// </summary>
public class Avatar : ControlBase
{
    /// <summary>
    /// Gets or sets the source of the avatar image.
    /// </summary>
    [Parameter]
    public string? Src { get; set; }

    /// <summary>
    /// The ID of the user which is used to source the avatar image.
    /// </summary>
    [Parameter]
    public string? UserId { get; set; }

    /// <summary>
    /// The size, in pixels, of the avatar image.
    /// </summary>
    [Parameter]
    public double? Size { get; set; }

    /// <summary>
    /// Gets or sets the source of the avatar image when an error occurs.
    /// </summary>
    /// <remarks>
    /// By default, the source is set to "/images/anonymous.svg".
    /// </remarks>
    [Parameter]
    public string? SrcOnError { get; set; } = "/images/anonymous.svg";

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add("avatar");
    }

    ///<inheritdoc/>
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "img");

        string? src = string.IsNullOrEmpty(UserId) ? Src : Resolver.ResolveAvatarId(UserId);

        builder.AddAttribute(1, "src", src);
        builder.AddAttribute(2, "height", Size ?? Resolver.DefaultAvatarSize);
        builder.AddAttribute(2, "width", Size ?? Resolver.DefaultAvatarSize);
        builder.AddAttribute(3, "onerror", $"this.src ='{SrcOnError}';this.onerror=''");
        builder.AddMultipleAttributes(4, GetAttributes());
        builder.CloseElement();
    }
}
