namespace Ocluse.LiquidSnow.Cryptography.Classical
{
    /// <summary>
    /// Used to determined the direction in the grid to move where for example, the same character forms the diagram, e.g. FF or KK
    /// </summary>
    public enum PreferredOrientation
    {
        /// <summary>
        /// Conflicts will be treated as if they occurred in the same row.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Conflicts will be treated as if they occurred in the same column
        /// </summary>
        Vertical
    }
}
