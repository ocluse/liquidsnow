using Ocluse.LiquidSnow.Venus.Components;

namespace Ocluse.LiquidSnow.Venus.Contracts;

/// <summary>
/// A contract used by Venus services to resolve the representation state of components.
/// </summary>
public interface IVenusResolver
{
    /// <summary>
    /// The default page size used by an <see cref="CollectionView{T}"/> component.
    /// </summary>
    int DefaultPageSize { get; }

    /// <summary>
    /// The default icon size used by a <see cref="Button"/> component.
    /// </summary>
    int DefaultButtonIconSize { get; }

    /// <summary>
    /// The default icon size used by a <see cref="FeatherIcon"/> or <see cref="FluentIcon"/> component.
    /// </summary>
    int DefaultIconSize { get; }

    /// <summary>
    /// The size for snackbar item status icon.
    /// </summary>
    int SnackbarIconSize { get; }

    /// <summary>
    /// The default size, in pixels, for the <see cref="Avatar"/> component
    /// </summary>
    int DefaultAvatarSize { get; }

    /// <summary>
    /// The stroke width applied to icon's that accept a stroke-width value.
    /// </summary>
    int IconStrokeWidth { get; }

    /// <summary>
    /// The icon style used by various components.
    /// </summary>
    IconStyle IconStyle { get; }

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
    int ResolveTextStyleToIconSize(int textStyle);
}
