namespace Ocluse.LiquidSnow.Entities
{
    /// <summary>
    /// Used as the return type for commands and queries that don't have a specific handler.
    /// </summary>
    public readonly struct Unit
    {
        /// <summary>
        /// The only value for unit.
        /// </summary>
        public static Unit Value { get; } = new Unit();
    }
}
