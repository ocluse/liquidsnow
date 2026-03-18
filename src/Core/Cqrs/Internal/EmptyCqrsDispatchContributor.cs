namespace Ocluse.LiquidSnow.Cqrs.Internal;

internal sealed class EmptyCqrsDispatchContributor : ICqrsDispatchContributor
{
    public IEnumerable<DispatchDescriptor> Descriptors => [];
}
