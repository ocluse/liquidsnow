using System;
using System.Collections.Generic;
using System.Text;

namespace Ocluse.LiquidSnow.Cryptography
{
    /// <summary>
    /// Represents a type of hash algorithms
    /// </summary>
    public enum HashAlgorithmKind
    {
        /// <summary>
        /// Uses the MD5 algorithm
        /// </summary>
        MD5,
        /// <summary>
        /// Uses the SHA256 algorithm
        /// </summary>
        Sha256,

        /// <summary>
        /// Uses the SHA512 algorithm
        /// </summary>
        Sha512
    }
}
