namespace Ocluse.LiquidSnow.Data;

public record LoadRequest<TKey>
{
    public required TKey? Key { get; init; }

    public required LoadType Type { get; init; }

    public required int PageSize { get; init; }
}
