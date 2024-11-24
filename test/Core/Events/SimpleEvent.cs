using Ocluse.LiquidSnow.Events;

namespace Ocluse.LiquidSnow.Core.Tests.Events;

public record SimpleEvent
{
    public required int InitialValue { get; init; }

    public required int Added { get; init; }

    public int Value { get; set; }
}

public class SimpleEventListener : IEventListener<SimpleEvent>
{
    public Task HandleAsync(SimpleEvent e, CancellationToken cancellationToken = default)
    {
        e.Value = e.InitialValue + e.Added;

        return Task.CompletedTask;
    }
}