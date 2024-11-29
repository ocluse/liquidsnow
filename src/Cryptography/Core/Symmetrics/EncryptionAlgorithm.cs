namespace Ocluse.LiquidSnow.Cryptography.Symmetrics;

/// <summary>
/// The symmetric cryptographic algorithm used by <see cref="ISymmetric"/>
/// </summary>
public enum EncryptionAlgorithm
{
    /// <summary>
    /// The Advanced Encryption Standard algorithm
    /// </summary>
    AES,

    /// <summary>
    /// The DES algorithm
    /// </summary>
    DES,

    /// <summary>
    /// The RC2 algorithm
    /// </summary>
    RC2,

    /// <summary>
    /// The TripleDES algorithm
    /// </summary>
    TripleDES
}