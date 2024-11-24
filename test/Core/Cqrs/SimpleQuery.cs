using Ocluse.LiquidSnow.Cqrs;

namespace Ocluse.LiquidSnow.Core.Tests.Cqrs;

public record SimpleQuery : IQuery<int>
{
    public required int Value { get; init; }
}

internal class SimpleQueryHandler : IQueryHandler<SimpleQuery, int>
{
    public Task<int> HandleAsync(SimpleQuery query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(query.Value);
    }
}