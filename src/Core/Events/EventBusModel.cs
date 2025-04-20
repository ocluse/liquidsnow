namespace Ocluse.LiquidSnow.Events;

public static class EventBusModel
{
    public static event Action<Exception>? UnobservedException;

    internal static void OnUnobservedException(Exception exception)
    {
        UnobservedException?.Invoke(exception);
    }
}