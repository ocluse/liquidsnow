﻿namespace Ocluse.LiquidSnow.Cryptography.Symmetrics
{
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
        /// <param name="algorithm">The algorithm used for the cryptographic operations</param>
        /// <param name="salt">The salt used in derivation of Key and IV during the cryptographic operation</param>
        /// <returns>An <see cref="ISymmetric"/> instance initialized with the specified algorithm and salt that can be used for cryptographic operations</returns>
        public static ISymmetric Create(EncryptionAlgorithm algorithm, string salt)
        {
            return new Symmetric()
            {
                KeySize = _keySize,
                Algorithm = algorithm,
                BlockSize = _blockSize,
                Salt = salt,
                CipherMode = System.Security.Cryptography.CipherMode.CBC,
                PaddingMode = System.Security.Cryptography.PaddingMode.PKCS7,
                Iterations = _iterations
            };
        }

        /// <summary>
        /// Creates a <see cref="ISymmetric"/> instance using the AES algorithm with a randomly generated salt. The salt must be stored for future use.
        /// </summary>
        /// <returns>An <see cref="ISymmetric"/> instance using the AES algorithm with a random salt.</returns>
        public static ISymmetric CreateAes(out string salt)
        {
            salt = CryptoUtility.GenerateId(IdKind.Standard, 16);
            return Create(EncryptionAlgorithm.Aes, salt);
        }

        /// <summary>
        /// Creates a <see cref="ISymmetric"/> instance using the Rijndael algorithm with a randomly generated salt. The salt must be stored for future use.
        /// </summary>
        /// <returns>An <see cref="ISymmetric"/> instance using the Rijndael algorithm with a random salt.</returns>
        public static ISymmetric CreateRijndael(out string salt)
        {
            salt = CryptoUtility.GenerateId(IdKind.Standard, 16);
            return Create(EncryptionAlgorithm.Rijndael, salt);
        }
        #endregion
    }
}
