namespace Ocluse.LiquidSnow.Data;

public class PagerStateChangedArgs(PagerState state) : EventArgs
{
    public PagerState State => state;
}
