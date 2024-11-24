using Ocluse.LiquidSnow.Cqrs;

namespace Ocluse.LiquidSnow.Core.Tests.Cqrs;

public class CqrsTests : IClassFixture<CqrsApplication>
{
    private readonly CqrsApplication _application;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher;
    private readonly Random _random;

    public CqrsTests(CqrsApplication application)
    {
        _random = new();
        _application = application;
        _commandDispatcher = _application.Host.Services.GetRequiredService<ICommandDispatcher>();
        _queryDispatcher = _application.Host.Services.GetRequiredService<IQueryDispatcher>();
    }

    [Fact]
    public async Task SimpleCommandTest()
    {
        int value = _random.Next(1, 100);

        SimpleCommand testCommand = new() { Value = value };

        var result = await _commandDispatcher.DispatchAsync(testCommand);

        Assert.Equal(value, result);
    }

    [Fact]
    public async Task PipelineCommandTest()
    {
        int initialValue = _random.Next(1, 100);
        int preprocessAddition = _random.Next(1, 100);
        int postprocessAddition = _random.Next(1, 100);
        int handlerAddition = _random.Next(1, 100);

        PipelineCommand pipelineCommand = new()
        {
            HandleAddition = handlerAddition,
            InitialValue = initialValue,
            PostprocessAddition = postprocessAddition,
            PreprocessAddition = preprocessAddition
        };

        var result = await _commandDispatcher.DispatchAsync(pipelineCommand);

        int expected = initialValue + preprocessAddition + handlerAddition + postprocessAddition;

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task SimpleQueryTest()
    {
        int value = _random.Next(1, 100);

        SimpleQuery testQuery = new() { Value = value };

        var result = await _queryDispatcher.DispatchAsync(testQuery);

        Assert.Equal(value, result);
    }

    [Fact]
    public async Task PipelineQueryTest()
    {
        int initialValue = _random.Next(1, 100);
        int preprocessAddition = _random.Next(1, 100);
        int postprocessAddition = _random.Next(1, 100);
        int handlerAddition = _random.Next(1, 100);

        PipelineQuery pipelineQuery = new()
        {
            HandleAddition = handlerAddition,
            InitialValue = initialValue,
            PostprocessAddition = postprocessAddition,
            PreprocessAddition = preprocessAddition
        };

        var result = await _queryDispatcher.DispatchAsync(pipelineQuery);

        int expected = initialValue + preprocessAddition + handlerAddition + postprocessAddition;

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task PolymorphicCommandTest()
    {
        int initialValue = _random.Next(1, 100);
        int additiveValue = _random.Next(1, 100);
        int subtractiveValue = _random.Next(1, 100);

        AdditivePolymorphicCommand additiveCommand = new()
        {
            Value = initialValue,
            Added = additiveValue
        };

        SubtractivePolymorphicCommand subtractiveCommand = new()
        {
            Value = initialValue,
            Subtracted = subtractiveValue
        };

        var additiveResult = await _commandDispatcher.DispatchAsync<PolymorphicCommand, int>(additiveCommand);
        var subtractiveResult = await _commandDispatcher.DispatchAsync<PolymorphicCommand, int>(subtractiveCommand);

        Assert.Equal(initialValue + additiveValue, additiveResult);
        Assert.Equal(initialValue - subtractiveValue, subtractiveResult);
    }

    [Fact]
    public async Task PolymorphicQueryTest()
    {
        int initialValue = _random.Next(1, 100);
        int additiveValue = _random.Next(1, 100);
        int subtractiveValue = _random.Next(1, 100);

        AdditivePolymorphicQuery additiveQuery = new()
        {
            Value = initialValue,
            Added = additiveValue
        };

        SubtractivePolymorphicQuery subtractiveQuery = new()
        {
            Value = initialValue,
            Subtracted = subtractiveValue
        };

        var additiveResult = await _queryDispatcher.DispatchAsync<IPolymorphicQuery, int>(additiveQuery);
        var subtractiveResult = await _queryDispatcher.DispatchAsync<IPolymorphicQuery, int>(subtractiveQuery);

        Assert.Equal(initialValue + additiveValue, additiveResult);
        Assert.Equal(initialValue - subtractiveValue, subtractiveResult);
    }
}