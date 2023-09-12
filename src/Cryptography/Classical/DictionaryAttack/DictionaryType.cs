namespace Ocluse.LiquidSnow.Cryptography.Classical.DictionaryAttack
{
    /// <summary>
    /// Used to determine the function a dictionary will be used for.
    /// </summary>
    public enum DictionaryType
    {
        /// <summary>
        /// The dictionary will be used to check if any sequence of characters in the attacked data match any meaningful words.
        /// </summary>
        Language,
        /// <summary>
        /// The dictionary will be used to provide keys used to attack a particular cyphertext.
        /// </summary>
        Key,
        /// <summary>
        /// The dictionary is used both as a source of language and to obtain attack keys.
        /// </summary>
        Combined
    };
}
