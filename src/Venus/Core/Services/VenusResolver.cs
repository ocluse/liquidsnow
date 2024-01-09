using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Venus.Contracts;
using Ocluse.LiquidSnow.Venus.Values;

namespace Ocluse.LiquidSnow.Venus.Services
{
    /// <summary>
    /// A basic implementation of <see cref="IVenusResolver"/> that resolves values to the default Venus values.
    /// </summary>
    public class VenusResolver : IVenusResolver
    {
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
}
