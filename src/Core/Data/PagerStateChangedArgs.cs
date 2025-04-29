namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Represents the arguments for the <see cref="Pager{TKey, TItem}.StateChanged"/> event.
/// </summary>
/// <param name="state"></param>
public class PagerStateChangedArgs(PagerState state) : EventArgs
{
    /// <summary>
    /// Returns the updated state of the pager.
    /// </summary>
    public PagerState State => state;
}
