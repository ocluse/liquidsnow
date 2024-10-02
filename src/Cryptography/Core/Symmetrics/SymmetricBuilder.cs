using Ocluse.LiquidSnow.Extensions;
using System.Security.Cryptography;

namespace Ocluse.LiquidSnow.Cryptography.Symmetrics;

/// <summary>
/// Used to create instances of <see cref="ISymmetric"/>
/// </summary>
public static class SymmetricBuilder
{
    #region Constants
    private const int _keySize = 256;
    private const int _blockSize = 128;
    private const int _iterations = 1000;
    #endregion

    #region Public Methods

    /// <summary>
    /// Creates a <see cref="ISymmetric"/> of the algorithm and salt
    /// </summary>
    public static ISymmetric Create(EncryptionAlgorithm algorithm, byte[] salt, HashAlgorithmName hash)
    {
        return new Symmetric()
        {
            KeySize = _keySize,
            Algorithm = algorithm,
            BlockSize = _blockSize,
            Salt = salt,
            CipherMode = CipherMode.CBC,
            PaddingMode = PaddingMode.PKCS7,
            Iterations = _iterations,
            Hash = hash
        };
    }

    /// <summary>
    /// Creates a <see cref="ISymmetric"/> i of the algorithm ans salt, using the <see cref="HashAlgorithmName.SHA256"/> hash by default.
    /// </summary>
    public static ISymmetric Create(EncryptionAlgorithm algorithm, byte[] salt)
    {
        return Create(algorithm, salt, HashAlgorithmName.SHA256);
    }

    /// <summary>
    /// Creates a <see cref="ISymmetric"/> instance using the AES algorithm with a randomly generated salt. The salt must be stored for future use.
    /// </summary>
    /// <remarks>
    /// The Salt is generated using <see cref="CryptoUtility.GenerateId(IdKind, int)"/> with the <see cref="IdKind.Guid"/> kind and the hashing algorithm used is <see cref="HashAlgorithmName.SHA256"/>.
    /// </remarks>
    public static ISymmetric CreateAes(out byte[] salt)
    {
        salt = CryptoUtility.GenerateId(IdKind.Guid).GetBytes();
        return Create(EncryptionAlgorithm.AES, salt, HashAlgorithmName.SHA256);
    }
    #endregion
}
