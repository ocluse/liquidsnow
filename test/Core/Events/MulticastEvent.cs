using Ocluse.LiquidSnow.Events;

namespace Ocluse.LiquidSnow.Core.Tests.Events;

public record MulticastEvent
{
    public required int FirstAdded { get; init; }

    public required int SecondAdded { get; init; }

    public int Value { get; set; }
}

public class FirstMulticastEventListener : IEventListener<MulticastEvent>
{
    public Task HandleAsync(MulticastEvent e, CancellationToken cancellationToken = default)
    {
        e.Value += e.FirstAdded;

        return Task.CompletedTask;
    }
}

public class SecondMulticastEventListener : IEventListener<MulticastEvent>
{
    public Task HandleAsync(MulticastEvent e, CancellationToken cancellationToken = default)
    {
        e.Value += e.SecondAdded;

        return Task.CompletedTask;
    }
}