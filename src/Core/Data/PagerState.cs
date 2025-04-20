namespace Ocluse.LiquidSnow.Data;

public record PagerState
{
    public required LoadState Refresh { get; init; }

    public required LoadState Append { get; init; }

    public required LoadState Prepend { get; init; }
}
