using Ocluse.LiquidSnow.Extensions;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Ocluse.LiquidSnow.Cryptography
{
    /// <summary>
    /// Contains utility methods for generating IDs, random values and computing hashes.
    /// </summary>
    public class CryptoUtility
    {
        #region Generation
        /// <summary>
        /// Generates an arguably unique string of characters to be used for unique identification.
        /// </summary>
        /// <remarks>
        /// The uniqueness strength boils down to the <see cref="IdKind"/> used. Some IDs may not be suitable for certain scenarios
        /// </remarks>
        /// <param name="kind">The kind of ID to be generated</param>
        /// <param name="length">The length of the string to be generated, only applicable for <see cref="IdKind.Standard"/> up to 36 characters and <see cref="IdKind.Random"/> for unlimited characters</param>
        public static string GenerateId(IdKind kind = IdKind.Guid, int length = 12)
        {
            return kind switch
            {
                IdKind.Standard => GenerateStandardId(length),
                IdKind.DateTime => GenerateDateTimeId(),
                IdKind.Guid => GenerateGuid(),
                IdKind.Hash => GenerateHashedId(),
                IdKind.Random => Random(length, true),
                IdKind.Numeric => RandomNumeric(length),
                _ =>throw new NotImplementedException("Unknown ID kind")
            };
        }

        private static string GenerateDateTimeId()
        {
            long ticks = DateTime.Now.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            string id = Convert.ToBase64String(bytes)
                                    .Replace('+', '_')
                                    .Replace('/', '-')
                                    .TrimEnd('=');
            return id.ToUpper();
        }

        private static string GenerateStandardId(int count)
        {
            StringBuilder builder = new StringBuilder();
            Enumerable
                .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(count)
                .ToList().ForEach(e => builder.Append(e));
            return builder.ToString();
        }

        private static string GenerateGuid()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }

        private static string GenerateHashedId()
        {
            string start = GenerateId();
            return GetHash(start);
        }
        #endregion

        #region Randomnization
        private static readonly Random _random = new Random();

        /// <summary>
        /// Generates a random integer
        /// </summary>
        /// <param name="min">The minimum inclusive value</param>
        /// <param name="max">The maximum exclusive value</param>
        /// <returns>A randomly generated integer</returns>
        public static int Random(int min, int max)
        {
            return _random.Next(min, max);
        }

        /// <summary>
        /// Generates a random double between 0.0 and 1.0
        /// </summary>
        public static double Random()
        {
            return _random.NextDouble();
        }

        /// <summary>
        /// Generates a random string of the specified <paramref name="length"/>
        /// </summary>
        /// <param name="length">The number of characters to include in the string</param>
        /// <param name="lowerCase">If true, returns lowercase characters, otherwise uppercase</param>
        /// <returns>A randomly generated string</returns>
        public static string Random(int length, bool lowerCase = false)
        {
            StringBuilder builder = new StringBuilder(length);

            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26;
            for (int i = 0; i < length; i++)
            {
                char @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();

        }

        /// <summary>
        /// Generates a random numeric string of specified <paramref name="length"/>
        /// </summary>
        /// <param name="length">The number of digits the string should have</param>
        /// <returns>A randomly generated numeric string</returns>
        public static string RandomNumeric(int length)
        {
            int min = Convert.ToInt32(Math.Pow(10, (length - 1)));
            int max = Convert.ToInt32(Math.Pow(10, length) - 1);

            return Random(min, max).ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Hash

        /// <summary>
        /// Computes the hash value of file
        /// </summary>
        /// <param name="path">The path of the file whose hash value is to be computed</param>
        /// <param name="algorithm">The algorithm to use when computing the hash</param>
        /// <param name="returnHexString">If true, a hexadecimal notated string will be return, else, a base64 string is returned instead</param>
        public static string ComputeFileHash(string path, HashAlgorithmKind algorithm = HashAlgorithmKind.Sha256, bool returnHexString = false)
        {
            byte[] bytes = ComputeFileHash(path, algorithm);

            return returnHexString ? GetHexString(bytes) : Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Computes the hash value of file
        /// </summary>
        /// <param name="path">The path of the file whose hash value is to be computed</param>
        /// <param name="algorithm">The algorithm to use when computing the hash</param>
        public static byte[] ComputeFileHash(string path, HashAlgorithmKind algorithm = HashAlgorithmKind.Sha256)
        {
            using FileStream stream = File.OpenRead(path);
            return GetHash(stream, algorithm);
        }

        /// <summary>
        /// Computes the hash of an array of bytes
        /// </summary>
        /// <param name="data">The data to be hashed</param>
        /// <param name="algorithm">The algorithm to use when computing the hash</param>
        public static byte[] GetHash(byte[] data, HashAlgorithmKind algorithm = HashAlgorithmKind.Sha256)
        {
            using MemoryStream msData = new MemoryStream(data);
            return GetHash(msData, algorithm);
        }

        /// <summary>
        /// Computes the hash of a stream
        /// </summary>
        /// <param name="inputStream">The input stream</param>
        /// <param name="algorithm">The algorithm to use when computing the hash</param>
        /// <exception cref="NotImplementedException"> When the algorithm is not implemented</exception>
        public static byte[] GetHash(Stream inputStream, HashAlgorithmKind algorithm = HashAlgorithmKind.Sha256)
        {
            using HashAlgorithm alg = algorithm switch
            {
                HashAlgorithmKind.MD5 => MD5.Create(),
                HashAlgorithmKind.Sha256 => SHA256.Create(),
                HashAlgorithmKind.Sha512 => SHA512.Create(),
                _ => throw new NotImplementedException("Unknown Hash Algorithm kind.")
            };

            return alg.ComputeHash(inputStream);
        }

        /// <summary>
        /// Computes the hash of a string
        /// </summary>
        /// <param name="input">The string to be hashed</param>
        /// <param name="algorithm">The algorithm to use when computing the hash</param>
        /// <param name="returnHexString">If true, a hexadecimal notated string will be return, else, a base64 string is returned instead</param>
        public static string GetHash(string input, HashAlgorithmKind algorithm = HashAlgorithmKind.Sha256, bool returnHexString = false)
        {
            byte[] bytes = GetHash(input.GetBytes<UTF8Encoding>(), algorithm);

            return returnHexString ? GetHexString(bytes) : Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Returns a hexadecimal notation string of the input.
        /// </summary>
        public static string GetHexString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder();

            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        #endregion
    }
}
