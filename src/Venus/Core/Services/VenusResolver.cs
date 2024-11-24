using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Venus.Components;
using Ocluse.LiquidSnow.Venus.Components.Internal.ContainerStates;

namespace Ocluse.LiquidSnow.Venus.Services;

/// <summary>
/// A basic implementation of <see cref="IVenusResolver"/> that resolves values to the default Venus values.
/// </summary>
public class VenusResolver : IVenusResolver
{
    ///<inheritdoc/>
    public virtual int DefaultPageSize => 10;

    ///<inheritdoc/>
    public virtual double DefaultButtonIconSize => DefaultSize.Size18;

    ///<inheritdoc/>
    public virtual double DefaultIconSize => DefaultSize.Size24;

    ///<inheritdoc/>
    public virtual double DefaultAvatarSize => DefaultSize.Size48;

    ///<inheritdoc/>
    public virtual double SnackbarIconSize => DefaultSize.Size18;

    ///<inheritdoc/>
    public virtual IconStyle IconStyle => IconStyle.Feather;

    ///<inheritdoc/>
    public virtual int IconStrokeWidth => FeatherIcon.STROKE_WIDTH;

    ///<inheritdoc/>
    public virtual int DefaultMaxPaginatorItems => 10;

    ///<inheritdoc/>
    public virtual double DefaultDebounceInterval => 500;

    ///<inheritdoc/>
    public virtual CssUnit DefaultGapUnit => CssUnit.Rem;

    ///<inheritdoc/>
    public virtual StrokeLineCap FeatherIconStrokeCap => StrokeLineCap.Round;

    ///<inheritdoc/>
    public virtual StrokeLineJoin FeatherIconStrokeLineJoin => StrokeLineJoin.Round;

    ///<inheritdoc/>
    public virtual CssUnit DefaultIconSizeUnit => CssUnit.Px;

    ///<inheritdoc/>
    public virtual CssUnit DefaultImageSizeUnit => CssUnit.Px;

    ///<inheritdoc/>
    public virtual string DefaultImageFallbackSrc => "/images/placeholder.svg";

    ///<inheritdoc/>
    public virtual string DefaultAvatarFallbackSrc => "/images/anonymous.svg";

    ///<inheritdoc/>
    public virtual Type ResolveContainerStateToComponentType(int containerState)
    {
        return containerState switch
        {
            ContainerState.Loading => typeof(Loading),
            ContainerState.NotFound => typeof(NotFound),
            ContainerState.Empty => typeof(Empty),
            ContainerState.Error => typeof(Error),
            ContainerState.Unauthorized => typeof(Unauthorized),
            ContainerState.ReauthenticationRequired => typeof(ReauthenticationRequired),
            _ => throw new NotImplementedException()
        };
    }

    ///<inheritdoc/>
    public virtual string ResolveSnackbarStatusToIcon(int status)
    {
        return IconStyle switch
        {
            IconStyle.Fluent => GetFluentIconForStatus(status),
            _ => GetFeatherIconForStatus(status)
        };
    }

    private static string GetFeatherIconForStatus(int status)
    {
        return status switch
        {
            MessageStatus.Error => ComponentIcons.Feather.Error,
            MessageStatus.Information => ComponentIcons.Feather.Information,
            MessageStatus.Success => ComponentIcons.Feather.Success,
            MessageStatus.Warning => ComponentIcons.Feather.Warning,
            _ => throw new NotImplementedException()
        };
    }

    private static string GetFluentIconForStatus(int status)
    {
        return status switch
        {
            MessageStatus.Error => ComponentIcons.Fluent.Error,
            MessageStatus.Information => ComponentIcons.Fluent.Information,
            MessageStatus.Success => ComponentIcons.Fluent.Success,
            MessageStatus.Warning => ComponentIcons.Fluent.Warning,
            _ => throw new NotImplementedException()
        };
    }

    ///<inheritdoc/>
    public virtual int ResolveSnackbarStatusToColor(int status)
    {
        return status switch
        {
            MessageStatus.Error => Color.Error,
            MessageStatus.Information => Color.Primary,
            MessageStatus.Success => Color.Success,
            MessageStatus.Warning => Color.Warning,
            _ => throw new NotImplementedException()
        };
    }

    ///<inheritdoc/>
    public virtual int ResolveSnackbarDurationToMilliseconds(SnackbarDuration duration)
    {
        return duration switch
        {
            SnackbarDuration.Short => 3000,
            SnackbarDuration.Medium => 5000,
            SnackbarDuration.Long => 8000,
            SnackbarDuration.Infinite => 0,
            _ => throw new NotImplementedException()
        };
    }

    ///<inheritdoc/>
    public string ResolveSnackbarStatusToClass(int status)
    {
        return status switch
        {
            MessageStatus.Error => "error",
            MessageStatus.Information => "information",
            MessageStatus.Success => "success",
            MessageStatus.Warning => "warning",
            _ => throw new NotImplementedException()
        };
    }

    ///<inheritdoc/>
    public virtual string ResolveAvatarId(string userId)
    {
        return $"https://cdn.ocluse.com/krystal/avatars/{userId}";
    }

    ///<inheritdoc/>
    public virtual string ResolveColor(int color)
    {
        string colorStr = color switch
        {
            Color.Primary => nameof(Color.Primary),
            Color.Secondary => nameof(Color.Secondary),
            Color.Tertiary => nameof(Color.Tertiary),
            Color.Error => nameof(Color.Error),
            Color.Warning => nameof(Color.Warning),
            Color.Success => nameof(Color.Success),
            Color.PrimaryContainer => nameof(Color.PrimaryContainer),
            Color.SecondaryContainer => nameof(Color.SecondaryContainer),
            Color.TertiaryContainer => nameof(Color.TertiaryContainer),
            Color.ErrorContainer => nameof(Color.ErrorContainer),
            Color.WarningContainer => nameof(Color.WarningContainer),
            Color.SuccessContainer => nameof(Color.SuccessContainer),
            Color.Background => nameof(Color.Background),
            Color.OnBackground => nameof(Color.OnBackground),
            Color.Surface => nameof(Color.Surface),
            Color.OnSurface => nameof(Color.OnSurface),
            Color.Outline => nameof(Color.Outline),
            Color.SurfaceVariant => nameof(Color.SurfaceVariant),
            Color.OnSurfaceVariant => nameof(Color.OnSurfaceVariant),
            _ => throw new NotImplementedException()
        };

        return $"var(--{colorStr.PascalToKebabCase()})";
    }

    ///<inheritdoc/>
    public virtual int ResolveExceptionToContainerState(Exception exception)
    {
        if (exception is UnauthorizedAccessException)
        {
            return ContainerState.Unauthorized;
        }
        else if (exception is HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return ContainerState.NotFound;
            }
            else if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return ContainerState.Unauthorized;
            }

        }
        return ContainerState.Error;
    }

    ///<inheritdoc/>
    public virtual string ResolveTextHierarchy(int textHierarchy)
    {
        string result = textHierarchy switch
        {
            TextHierarchy.H1 => nameof(TextHierarchy.H1),
            TextHierarchy.H2 => nameof(TextHierarchy.H2),
            TextHierarchy.H3 => nameof(TextHierarchy.H3),
            TextHierarchy.H4 => nameof(TextHierarchy.H4),
            TextHierarchy.H5 => nameof(TextHierarchy.H5),
            TextHierarchy.H6 => nameof(TextHierarchy.H6),
            TextHierarchy.Span => nameof(TextHierarchy.Span),
            TextHierarchy.P => nameof(TextHierarchy.P),
            TextHierarchy.DFN => nameof(TextHierarchy.DFN),
            _ => throw new NotImplementedException()
        };

        return result.ToLowerInvariant();
    }

    ///<inheritdoc/>
    public virtual string ResolveTextStyle(int textStyle)
    {
        string style = textStyle switch
        {
            TextStyle.Caption => nameof(TextStyle.Caption),
            TextStyle.CaptionStrong => nameof(TextStyle.CaptionStrong),
            TextStyle.Body => nameof(TextStyle.Body),
            TextStyle.BodyStrong => nameof(TextStyle.BodyStrong),
            TextStyle.BodyLarge => nameof(TextStyle.BodyLarge),
            TextStyle.Subtitle => nameof(TextStyle.Subtitle),
            TextStyle.Title => nameof(TextStyle.Title),
            TextStyle.TitleLarge => nameof(TextStyle.TitleLarge),
            TextStyle.Display => nameof(TextStyle.Display),
            _ => throw new NotImplementedException()
        };

        return $"text-{style.PascalToKebabCase()}";
    }

    ///<inheritdoc/>
    public int ResolveTextStyleToIconSize(int textStyle)
    {
        return textStyle switch
        {
            TextStyle.Caption => DefaultSize.Size14,
            TextStyle.Body => DefaultSize.Size16,
            TextStyle.BodyLarge => DefaultSize.Size18,
            TextStyle.Subtitle => DefaultSize.Size24,
            TextStyle.Title => DefaultSize.Size32,
            TextStyle.TitleLarge => DefaultSize.Size36,
            TextStyle.Display => DefaultSize.Size48,
            _ => DefaultSize.Size18
        };
    }
}
