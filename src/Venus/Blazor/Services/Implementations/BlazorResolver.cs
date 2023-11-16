using Ocluse.LiquidSnow.Venus.Blazor.Components;
using Ocluse.LiquidSnow.Venus.Blazor.Components.ContainerStates;

namespace Ocluse.LiquidSnow.Venus.Blazor.Services.Implementations
{
    public class BlazorResolver : IBlazorResolver
    {
        public virtual int DefaultPageSize => 10;

        public virtual int DefaultButtonIconSize => DefaultSize.Size18;

        public virtual int DefaultIconSize => DefaultSize.Size24;

        public virtual int DefaultAvatarSize => DefaultSize.Size48;

        public virtual int SnackbarIconSize => DefaultSize.Size18;

        public virtual IconStyle IconStyle => IconStyle.Feather;

        public virtual int DefaultFeatherStokeWidth => FeatherIcon.STROKE_WIDTH;

        public virtual Type ResolveContainerStateToRenderType(int containerState)
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
    }
}
