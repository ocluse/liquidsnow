using Ocluse.LiquidSnow.Cqrs;

internal record PipelineQuery : IQuery<int>
{
    public required int InitialValue { get; init; }

    public required int HandleAddition { get; init; }

    public required int PreprocessAddition { get; init; }

    public required int PostprocessAddition { get; init; }

    public int Value { get; set; }
}

internal class PipelineQueryPreprocessor : IQueryPreprocessor<PipelineQuery, int>
{
    public Task<PipelineQuery> HandleAsync(PipelineQuery Query, CancellationToken cancellationToken = default)
    {
        Query.Value = Query.InitialValue + Query.PreprocessAddition;

        return Task.FromResult(Query);
    }
}

internal class PipelineQueryHandler : IQueryHandler<PipelineQuery, int>
{
    public Task<int> HandleAsync(PipelineQuery Query, CancellationToken cancellationToken = default)
    {
        Query.Value += Query.HandleAddition;

        return Task.FromResult(Query.Value);
    }
}

internal class PipelineQueryPostprocessor : IQueryPostprocessor<PipelineQuery, int>
{
    public Task<int> HandleAsync(PipelineQuery Query, int result, CancellationToken cancellationToken = default)
    {
        var finalResult = result + Query.PostprocessAddition;
        return Task.FromResult(finalResult);
    }
}