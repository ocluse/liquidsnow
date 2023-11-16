namespace Ocluse.LiquidSnow.Venus.Blazor.Services
{
    public interface IBlazorResolver
    {
        int DefaultPageSize { get; }

        int DefaultButtonIconSize { get; }

        int DefaultIconSize { get; }

        int SnackbarIconSize { get; }

        int DefaultAvatarSize { get; }

        int DefaultFeatherStokeWidth { get; }

        IconStyle IconStyle { get; }

        Type ResolveContainerStateToRenderType(int containerState);
        
        string ResolveSnackbarStatusToIcon(int status);
        
        int ResolveSnackbarDurationToMilliseconds(SnackbarDuration duration);
        
        int ResolveSnackbarStatusToColor(int status);

        string ResolveSnackbarStatusToClass(int status);
    }
}
