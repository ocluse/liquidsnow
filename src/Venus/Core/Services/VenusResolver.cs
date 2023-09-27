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
                Color.Primary => nameof(Color.Primary).PascalToKebabCase(),
                Color.Secondary => nameof(Color.Secondary).PascalToKebabCase(),
                Color.Tertiary => nameof(Color.Tertiary).PascalToKebabCase(),
                Color.Error => nameof(Color.Error).PascalToKebabCase(),
                Color.Warning => nameof(Color.Warning).PascalToKebabCase(),
                Color.Success => nameof(Color.Success).PascalToKebabCase(),
                Color.PrimaryContainer => nameof(Color.PrimaryContainer).PascalToKebabCase(),
                Color.SecondaryContainer => nameof(Color.SecondaryContainer).PascalToKebabCase(),
                Color.TertiaryContainer => nameof(Color.TertiaryContainer).PascalToKebabCase(),
                Color.ErrorContainer => nameof(Color.ErrorContainer).PascalToKebabCase(),
                Color.WarningContainer => nameof(Color.WarningContainer).PascalToKebabCase(),
                Color.SuccessContainer => nameof(Color.SuccessContainer).PascalToKebabCase(),
                Color.Background => nameof(Color.Background).PascalToKebabCase(),
                Color.OnBackground => nameof(Color.OnBackground).PascalToKebabCase(),
                Color.Surface => nameof(Color.Surface).PascalToKebabCase(),
                Color.OnSurface => nameof(Color.OnSurface).PascalToKebabCase(),
                Color.Outline => nameof(Color.Outline).PascalToKebabCase(),
                Color.SurfaceVariant => nameof(Color.SurfaceVariant).PascalToKebabCase(),
                Color.OnSurfaceVariant => nameof(Color.OnSurfaceVariant).PascalToKebabCase(),
                _ => throw new NotImplementedException()
            };

            return $"var(--color-{colorStr})";
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
            return textHierarchy switch
            {
                TextHierarchy.H1 => nameof(TextHierarchy.H1).ToLower(),
                TextHierarchy.H2 => nameof(TextHierarchy.H2).ToLower(),
                TextHierarchy.H3 => nameof(TextHierarchy.H3).ToLower(),
                TextHierarchy.H4 => nameof(TextHierarchy.H4).ToLower(),
                TextHierarchy.H5 => nameof(TextHierarchy.H5).ToLower(),
                TextHierarchy.H6 => nameof(TextHierarchy.H6).ToLower(),
                TextHierarchy.Span => nameof(TextHierarchy.Span).ToLower(),
                TextHierarchy.P => nameof(TextHierarchy.P).ToLower(),
                TextHierarchy.DFN => nameof(TextHierarchy.DFN).ToLower(),
                _ => throw new NotImplementedException()
            };
        }

        ///<inheritdoc/>
        public virtual string ResolveTextStyle(int textStyle)
        {
            string style = textStyle switch
            {
                TextStyle.Caption => nameof(TextStyle.Caption).PascalToKebabCase(),
                TextStyle.CaptionStrong => nameof(TextStyle.CaptionStrong).PascalToKebabCase(),
                TextStyle.Body => nameof(TextStyle.Body).PascalToKebabCase(),
                TextStyle.BodyStrong => nameof(TextStyle.BodyStrong).PascalToKebabCase(),
                TextStyle.BodyLarge => nameof(TextStyle.BodyLarge).PascalToKebabCase(),
                TextStyle.Subtitle => nameof(TextStyle.Subtitle).PascalToKebabCase(),
                TextStyle.Title => nameof(TextStyle.Title).PascalToKebabCase(),
                TextStyle.TitleLarge => nameof(TextStyle.TitleLarge).PascalToKebabCase(),
                TextStyle.Display => nameof(TextStyle.Display).PascalToKebabCase(),
                _ => throw new NotImplementedException()
            };

            return $"text-{style}";
        }
    }
}
