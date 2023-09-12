namespace Ocluse.LiquidSnow.Cryptography.Classical.DictionaryAttack
{
    /// <summary>
    /// Defines a possible key and it's corresponding output.
    /// </summary>
    public class AttackPossibility
    {
        /// <summary>
        /// The key that was used in creating the <see cref="AttackPossibility"/>.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The output obtained when using the key for the <see cref="AttackPossibility"/>.
        /// </summary>
        public string Output { get; }

        /// <summary>
        /// Creates a new instance of <see cref="AttackPossibility"/>.
        /// </summary>
        public AttackPossibility(string key, string output)
        {
            Key = key;
            Output = output;
        }

        /// <summary>
        /// Returns a string of the possibility containing the key and possible output.
        /// </summary>
        public override string ToString()
        {
            return $"Key: {Key} Output: {Output}";

        }
    }
}
