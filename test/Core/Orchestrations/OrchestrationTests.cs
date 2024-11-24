using Ocluse.LiquidSnow.Orchestrations;

namespace Ocluse.LiquidSnow.Core.Tests.Orchestrations;

public class OrchestrationTests : IClassFixture<OrchestrationApplication>
{
    private readonly OrchestrationApplication _application;
    private readonly IOrchestrator _orchestrator;
    private readonly Random _random = new();
    public OrchestrationTests(OrchestrationApplication application)
    {
        _application = application;
        _orchestrator = _application.Host.Services.GetRequiredService<IOrchestrator>();
    }

    [Fact]
    public async Task SimpleOrchestrationTest()
    {
        int firstValue = _random.Next(1, 100);
        int secondValue = _random.Next(1, 100);
        
        SimpleOrchestration orchestration = new()
        {
            AddStepOne = firstValue,
            AddStepTwo = secondValue
        };

        int actualValue = await _orchestrator.ExecuteAsync(orchestration);

        int expectedValue = firstValue + secondValue;

        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, false, true)]
    [InlineData(false, true, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, false)]
    [InlineData(true, false, true)]
    [InlineData(true, true, false)]
    [InlineData(true, true, true)]
    public async Task ConditionalOrchestrationTest(bool executeFirst, bool executeSecond, bool produceGoTo)
    {        
        ConditionalOrchestration conditional = new()
        {
            AddIfFirstCondition = _random.Next(1, 100),
            AddIfSecondCondition = _random.Next(1, 100),
            AddInJumpedStep = _random.Next(1, 100),
            AddPreprocessStep = _random.Next(1, 100),
            AddPostprocessStep = _random.Next(1, 100),
            ExecuteFirstCondition = executeFirst,
            ExecuteSecondCondition = executeSecond,
            ProduceGoToResultInPreprocess = produceGoTo,
        };

        int expectedValue = conditional.AddPreprocessStep;

        if (!produceGoTo)
        {
            expectedValue += conditional.AddInJumpedStep;
        }

        if(executeFirst)
        {
            expectedValue += conditional.AddIfFirstCondition;
        }

        if (executeSecond)
        {
            expectedValue += conditional.AddIfSecondCondition;
        }

        expectedValue += conditional.AddPostprocessStep;

        int actualValue = await _orchestrator.ExecuteAsync(conditional);

        Assert.Equal(expectedValue, actualValue);
    }
}
