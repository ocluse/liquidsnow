namespace Ocluse.LiquidSnow.Events.Internal;

internal sealed class EmptyEventDispatchContributor : IEventDispatchContributor
{
    public IEnumerable<EventDispatchDescriptor> Descriptors => [];
}
