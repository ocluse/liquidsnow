using System;
using System.Linq;
using System.Text;

namespace Ocluse.LiquidSnow.Extensions
{
    /// <summary>
    /// Extensions methods for global types
    /// </summary>
    public static class GlobalExtensions
    {
        /// <summary>
        /// Returns the largest prime factor of a number
        /// </summary>
        public static ulong MaxFactor(this ulong n)
        {
            unchecked
            {
                while (n > 3 && 0 == (n & 1)) n >>= 1;

                uint k = 3;
                ulong k2 = 9;
                ulong delta = 16;
                while (k2 <= n)
                {
                    if (n % k == 0)
                    {
                        n /= k;
                    }
                    else
                    {
                        k += 2;
                        if (k2 + delta < delta) return n;
                        k2 += delta;
                        delta += 8;
                    }
                }
            }

            return n;
        }

        /// <summary>
        /// Quickly check if a character array is the same as the input string
        /// </summary>
        /// <param name="chars">An array of characters to determine if is equal to a string</param>
        /// <param name="value">The string to see if is same as the character array</param>
        /// <returns>true if the character array is equal to the provided string</returns>
        public static bool IsString(this char[] chars, string value)
        {
            var test = new string(chars);
            return test == value;
        }

        /// <summary>
        /// Quickly tests if a string is similar to a character array.
        /// </summary>
        /// <param name="value">The string to check if is equal to an array of characters</param>
        /// <param name="chars">The character array to test whether is the same as the string</param>
        /// <returns>true if the string is equal to the character array</returns>
        public static bool IsCharArray(this string? value, char[] chars)
        {
            var test = new string(chars);
            return test == value;
        }

        /// <summary>
        /// Checks if a double is a perfect square
        /// </summary>
        /// <param name="input"></param>
        /// <returns>true if a double is a perfect square, i.e the squareroot is an integer.</returns>
        public static bool IsPerfectSquare(this double input)
        {
            var sqrt = Math.Sqrt(input);
            return Math.Abs(Math.Ceiling(sqrt) - Math.Floor(sqrt)) < double.Epsilon;
        }

        /// <summary>Similar to <see cref="string.Substring(int,int)"/>, only for arrays. Returns a new
        /// array containing <paramref name="length"/> items from the specified
        /// <paramref name="startIndex"/> onwards.</summary>
        public static T[] Subarray<T>(this T[] array, int startIndex, int length)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex", "startIndex cannot be negative.");
            if (length < 0 || startIndex + length > array.Length)
                throw new ArgumentOutOfRangeException("length", "length cannot be negative or extend beyond the end of the array.");
            T[] result = new T[length];
            Array.Copy(array, startIndex, result, 0, length);
            return result;
        }

        /// <summary>
        /// Gets the bytes of a string when encoded using UTF8
        /// </summary>
        public static byte[] GetBytes(this string? str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// Gets the bytes of a string when encoded using the specified method
        /// </summary>
        /// <typeparam name="T">The type of Encoding to be used</typeparam>
        /// <param name="str">The string whose bytes are to be returned</param>
        public static byte[] GetBytes<T>(this string? str) where T : Encoding
        {
            if (typeof(T) == typeof(ASCIIEncoding))
            {
                return Encoding.ASCII.GetBytes(str);
            }
            else if (typeof(T) == typeof(UTF8Encoding))
            {
                return Encoding.UTF8.GetBytes(str);
            }
            else if (typeof(T) == typeof(UTF7Encoding))
            {
                return Encoding.UTF7.GetBytes(str);
            }
            else if (typeof(T) == typeof(UTF32Encoding))
            {
                return Encoding.UTF32.GetBytes(str);
            }
            else if (typeof(T) == typeof(UnicodeEncoding))
            {
                return Encoding.Unicode.GetBytes(str);
            }

            throw new ArgumentException("The encoding provided is unkown/unsupported");
        }

        /// <summary>
        /// Gets the string represented by a byte array using UTF8 encoding
        /// </summary>
        /// <returns>A string represented by the encoding in bytes</returns>
        public static string GetString(this byte[] ba)
        {
            return Encoding.UTF8.GetString(ba);
        }

        /// <summary>
        /// Gets the string represented by a byte array
        /// </summary>
        /// <typeparam name="T">The encoding to use</typeparam>
        /// <returns>A string represented by the encoding in the provided bytes</returns>
        public static string GetString<T>(this byte[] ba) where T : Encoding, new()
        {
            if (typeof(T) == typeof(ASCIIEncoding))
            {
                return Encoding.ASCII.GetString(ba);
            }
            else if (typeof(T) == typeof(UTF8Encoding))
            {
                return Encoding.UTF8.GetString(ba);
            }
            else if (typeof(T) == typeof(UTF7Encoding))
            {
                return Encoding.UTF7.GetString(ba);
            }
            else if (typeof(T) == typeof(UTF32Encoding))
            {
                return Encoding.UTF32.GetString(ba);
            }
            else if (typeof(T) == typeof(UnicodeEncoding))
            {
                return Encoding.Unicode.GetString(ba);
            }
            throw new ArgumentException("The encoding provided is unknown/unsupported");
        }

        /// <summary>
        /// Converts an integer to it's equivalent byte array.
        /// </summary>
        /// <returns>A byte array represented by the int.</returns>
        public static byte[] GetBytes(this int val)
        {
            return BitConverter.GetBytes(val);
        }

        /// <summary>
        /// Converts a byte array to it's equivalent byte array.
        /// </summary>
        /// <returns>An int representing the byte array</returns>
        public static int GetInt(this byte[] bytes)
        {
            var i = BitConverter.ToInt32(bytes, 0);
            return i;
        }

        ///<inheritdoc cref="GetPropValue{T}(object, string)"/>
        public static object GetPropValue(this object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            return prop.GetValue(obj);
        }

        /// <summary>
        /// Gets the value of a property with the provided name in the object.
        /// </summary>
        /// <typeparam name="T">The type to cast the property to</typeparam>
        /// <param name="obj">The object whose property is to be obtained</param>
        /// <param name="propertyName">The name of the property to retrieve</param>
        /// <returns>The value of the property otained from the object</returns>
        public static T GetPropValue<T>(this object obj, string propertyName)
        {
            return (T)obj.GetPropValue(propertyName);
        }

        /// <summary>
        /// Sets the value of a property with the provided name to the provided value
        /// </summary>
        /// <param name="obj">The object whose property is to be set</param>
        /// <param name="value">The value to set to the property</param>
        /// <param name="propertyName">The name of the property</param>
        public static void SetPropValue(this object obj, string propertyName, object value)
        {
            var prop = obj.GetType().GetProperty(propertyName);
            prop.SetValue(obj, value);
        }

        /// <summary>
        /// Checks if a string is composed of letters and digits only.
        /// </summary>
        /// <param name="s"></param>
        /// <returns>True if a string is composed of letters and numbers only.</returns>
        public static bool IsAlphaNumeric(this string? s)
        {
            return s.All(char.IsLetterOrDigit);
        }

        /// <summary>
        /// Checks whether the bytes of this array are the same as those of the second array
        /// </summary>
        /// <param name="a1">The source array</param>
        /// <param name="array">The array to check</param>
        /// <returns>True if the bytes are the same</returns>
        public static bool Compare(this byte[] a1, byte[] array)
        {
            if (a1.Length != array.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != array[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Checks if a character exists in a string.
        /// </summary>
        /// <returns>
        /// True if the character exists in the string.
        /// </returns>
        public static bool Contains(this string str, char c)
        {
            foreach (var ch in str)
            {
                if (ch == c) return true;
            }
            return false;
        }

        /// <summary>
        /// Returns a neatly formatted string representing the size of a file/stream e.g 8MB.
        /// </summary>
        /// <param name="size">The size of the file in bytes</param>
        /// <param name="decimals">The number of decimal places that can be displayed</param>
        public static string ToFileSizeReadable(this long size, uint decimals = 2)
        {
            // Get absolute value
            long absolute_i = size < 0 ? -size : size;
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = size >> 50;
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = size >> 40;
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = size >> 30;
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = size >> 20;
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = size >> 10;
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = size;
            }
            else
            {
                return size.ToString("0B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable /= 1024;
            // Return formatted number with suffix

            if (decimals == 0)
            {
                return $"{(int)readable}{suffix}";
            }

            StringBuilder builder = new StringBuilder("0.");

            for (int i = 0; i < decimals; i++)
            {
                builder.Append('#');
            }
            return $"{readable.ToString(builder.ToString())}{suffix}";
        }
    }
}
