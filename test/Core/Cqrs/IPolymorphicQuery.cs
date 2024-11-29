using Ocluse.LiquidSnow.Cqrs;

namespace Ocluse.LiquidSnow.Core.Tests.Cqrs;

public interface IPolymorphicQuery : IQuery<int>
{
    int Value { get; }
}

public record AdditivePolymorphicQuery : IPolymorphicQuery
{
    public int Added { get; init; }

    public int Value { get; init; }
}

public record SubtractivePolymorphicQuery : IPolymorphicQuery
{
    public int Subtracted { get; init; }

    public int Value { get; init; }
}

public class PolymorphicQueryHandler : IQueryHandler<IPolymorphicQuery, int>
{
    public Task<int> HandleAsync(IPolymorphicQuery Query, CancellationToken cancellationToken = default)
    {
        int result = Query.Value;

        if (Query is AdditivePolymorphicQuery additiveQuery)
        {
            result += additiveQuery.Added;
        }
        else if (Query is SubtractivePolymorphicQuery subtractiveQuery)
        {
            result -= subtractiveQuery.Subtracted;
        }
        else
        {
            throw new InvalidOperationException("Unknown Query type.");
        }

        return Task.FromResult(result);
    }
}