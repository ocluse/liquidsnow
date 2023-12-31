﻿using Ocluse.LiquidSnow.Extensions;
using System.Globalization;
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
            StringBuilder builder = new();
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
            var result = SHA256.HashData(start.GetBytes());
            return Convert.ToHexString(result);
        }
        #endregion

        #region Randomnization
        private static readonly Random _random = new();

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
            StringBuilder builder = new(length);

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

    }
}
