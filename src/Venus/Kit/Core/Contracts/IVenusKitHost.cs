namespace Ocluse.LiquidSnow.Venus.Kit.Contracts;

public interface IVenusKitHost
{
    event Action<VenusKitIntent>? IntentReceived;

    VenusKitIntent? LaunchIntent { get; }

    void RequestExit();
}
