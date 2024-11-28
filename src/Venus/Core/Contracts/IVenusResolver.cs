using Ocluse.LiquidSnow.Venus.Components;

namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// A contract used by Venus services to resolve the representation state of components.
/// </summary>
public interface IVenusResolver
{
    /// <summary>
    /// Gets the default page size used by an <see cref="CollectionViewBase{T}"/> component.
    /// </summary>
    int DefaultPageSize { get; }

    /// <summary>
    /// Gets the default maximum number of pagination items that can be displayed.
    /// </summary>
    int DefaultMaxPaginatorItems { get; }

    /// <summary>
    /// Gets the default icon size used by a <see cref="Button"/> component.
    /// </summary>
    double DefaultButtonIconSize { get; }

    /// <summary>
    /// Gets the default icon size used by a <see cref="FeatherIcon"/> or <see cref="FluentIcon"/> component.
    /// </summary>
    double DefaultIconSize { get; }

    /// <summary>
    /// Gets the default unit used by icon sizes.
    /// </summary>
    CssUnit DefaultIconSizeUnit { get; }

    /// <summary>
    /// Gets the size for snackbar item status icon.
    /// </summary>
    double SnackbarIconSize { get; }

    /// <summary>
    /// Gets the maximum number of snackbar items that can be displayed.
    /// </summary>
    int MaxSnackbarItems { get; }

    /// <summary>
    /// Gets the default size, in pixels, for the <see cref="Avatar"/> component
    /// </summary>
    double DefaultAvatarSize { get; }

    /// <summary>
    /// Gets or set the default unit used by images for size.
    /// </summary>
    CssUnit DefaultImageSizeUnit { get; }

    /// <summary>
    /// Gets the default fallback image src used when loading images fail.
    /// </summary>
    string DefaultImageFallbackSrc { get; }

    /// <summary>
    /// Gets or sets the default fallback image src used by avatars when loading images fail.
    /// </summary>
    string DefaultAvatarFallbackSrc { get; }

    /// <summary>
    /// Gets the default debounce interval in milliseconds used by input components.
    /// </summary>
    double DefaultDebounceInterval { get; }

    /// <summary>
    /// Gets the default CSS unit for gaps used by grid components.
    /// </summary>
    CssUnit DefaultGapUnit { get; }

    /// <summary>
    /// Gets the default CSS unit used for spacing values (paddings and margins).
    /// </summary>
    CssUnit DefaultSpacingUnit { get; }

    /// <summary>
    /// The stroke width applied to icon's that accept a stroke-width value.
    /// </summary>
    double DefaultFeatherIconStrokeWidth { get; }

    /// <summary>
    /// Gets the default stroke line cap for Feather icons.
    /// </summary>
    StrokeLineCap FeatherIconStrokeCap { get; }

    /// <summary>
    /// Gets the default stroke line join for Feather icons.
    /// </summary>
    StrokeLineJoin FeatherIconStrokeLineJoin { get; }

    /// <summary>
    /// Gets the icon style used by various components.
    /// </summary>
    IconStyle IconStyle { get; }

    /// <summary>
    /// Gets the default field header style used by <see cref="FieldBase{TValue}"/> components.
    /// </summary>
    FieldHeaderStyle DefaultFieldHeaderStyle { get; }

    /// <summary>
    /// Returns the component type that should be rendered for the given container state.
    /// </summary>
    Type ResolveContainerStateToComponentType(int containerState);

    /// <summary>
    /// Returns the icon that should be rendered for the given container state.
    /// </summary>
    string ResolveSnackbarStatusToIcon(int status);

    /// <summary>
    /// Returns the duration in milliseconds for the given snackbar duration.
    /// </summary>
    int ResolveSnackbarDurationToMilliseconds(SnackbarDuration duration);

    /// <summary>
    /// Returns the appropriate color to be applied for the given status.
    /// </summary>
    int ResolveSnackbarStatusToColor(int status);

    /// <summary>
    /// Returns the CSS class that will be added to a snackbar item for the given status.
    /// </summary>
    string ResolveSnackbarStatusToClass(int status);

    /// <summary>
    /// Returns the actual css style representation of the given color.
    /// </summary>
    string ResolveColor(int color);

    /// <summary>
    /// Returns the css class that will be applied for the given text.
    /// </summary>
    string ResolveTextStyle(int textStyle);

    /// <summary>
    /// Returns the actual HTML tag that will be used for the given text hierarchy.
    /// </summary>
    string ResolveTextHierarchy(int textHierarchy);

    /// <summary>
    /// Returns a URL to the image that will be used as the user's avatar.
    /// </summary>
    string ResolveAvatarId(string userId);

    /// <summary>
    /// Returns an int representing the container state to be rendered for the given exception.
    /// </summary>
    int ResolveExceptionToContainerState(Exception exception);

    /// <summary>
    /// Returns the appropriate icon size for the given text style.
    /// </summary>
    double ResolveTextStyleToIconSize(int textStyle);
}
