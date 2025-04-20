namespace Ocluse.LiquidSnow.Data;

public record LoadResult<TKey, TItem>
{
    public TKey? NextKey { get; init; }

    public TKey? PreviousKey { get; init; }

    public required IReadOnlyList<TItem> Items { get; init; }
}
