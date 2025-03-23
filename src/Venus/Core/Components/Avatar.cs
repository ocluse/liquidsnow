using Ocluse.LiquidSnow.Extensions;

namespace Ocluse.LiquidSnow.Venus.Components;

/// <summary>
/// A control for showing avatar images.
/// </summary>
public class Avatar : ImageBase
{
    /// <summary>
    /// Gets or sets the ID of the user which is used to source the image.
    /// </summary>
    [Parameter]
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the size of the image.
    /// </summary>
    [Parameter]
    public double? Size { get; set; }

    ///<inheritdoc/>
    protected override double? GetHeight() => Size ?? Resolver.DefaultAvatarSize;

    ///<inheritdoc/>
    protected override double? GetWidth() => Size ?? Resolver.DefaultAvatarSize;

    ///<inheritdoc/>
    protected override string GetDefaultFallbackSrc() => Resolver.DefaultAvatarFallbackSrc;

    ///<inheritdoc/>
    protected override bool GetDefaultUseFallbackForEmptySrc() => Resolver.DefaultUseFallbackForEmptyAvatarSrc;

    ///<inheritdoc/>
    protected override string? GetSource() => UserId.IsNotWhiteSpace() ? Resolver.ResolveAvatarId(UserId) : Src;

    ///<inheritdoc/>
    protected override void BuildClass(ClassBuilder classBuilder)
    {
        base.BuildClass(classBuilder);
        classBuilder.Add(ClassNameProvider.Avatar);
    }    
}
