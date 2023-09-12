namespace Ocluse.LiquidSnow.Cryptography.Symmetrics
{
    /// <summary>
    /// The symmetric cryptographic algorithm used by <see cref="ISymmetric"/>
    /// </summary>
    public enum EncryptionAlgorithm
    {
        /// <summary>
        /// The Advanced Encryption Standard algorithm
        /// </summary>
        Aes,

        /// <summary>
        /// The Rijndael algorithm
        /// </summary>
        Rijndael
    }
}