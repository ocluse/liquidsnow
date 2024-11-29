using Ocluse.LiquidSnow.Events;

namespace Ocluse.LiquidSnow.Core.Tests.Events;

public class EventsTests : IClassFixture<EventsApplication>
{
    private readonly EventsApplication _application;
    private readonly Random _random = new();
    private readonly IEventBus _eventBus;
    public EventsTests(EventsApplication application)
    {
        _application = application;
        _eventBus = _application.Host.Services.GetRequiredService<IEventBus>();
    }

    [Fact]
    public async Task SimpleEventTest()
    {
        int initial = _random.Next(1, 100);
        int added = _random.Next(1, 100);
        
        SimpleEvent e = new()
        {
            Added = added,
            InitialValue = initial
        };

        await _eventBus.PublishAsync(e);

        Assert.Equal(initial + added, e.Value);
    }

    [Fact]
    public async Task MulticastEventTest()
    {
        int value = _random.Next(1, 100);
        int firstAdded = _random.Next(1, 100);
        int secondAdded = _random.Next(1, 100);

        MulticastEvent e = new()
        {
            FirstAdded = firstAdded,
            SecondAdded = secondAdded,
            Value = value
        };

        await _eventBus.PublishAsync(e);

        Assert.Equal(value + firstAdded + secondAdded, e.Value);
    }

    [Fact]
    public async Task ServiceLifetimeTest()
    {
        using var scope = _application.Host.Services.CreateScope();

        DisposableService disposableService = scope.ServiceProvider.GetRequiredService<DisposableService>();

        await disposableService.DoSomethingAsync();

        scope.Dispose();

        ServiceLifetimeTestingEvent e = new();

        _eventBus.Publish(e);
        
        await Task.Delay(ServiceLifetimeTestingEvent.DELAY_MILI * 2);

        Assert.True(e.Success);
    }
}
