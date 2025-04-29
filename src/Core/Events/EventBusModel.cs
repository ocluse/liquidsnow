namespace Ocluse.LiquidSnow.Events;

/// <summary>
/// A utility class that can provide events globally for exceptions occurring on unobserved/fire and forget event publishes.
/// </summary>
public static class EventBusModel
{
    /// <summary>
    /// An event that is raised when an unobserved exception occurs in relation to event publishing.
    /// </summary>
    public static event Action<Exception>? UnobservedException;

    internal static void OnUnobservedException(Exception exception)
    {
        UnobservedException?.Invoke(exception);
    }
}