using Ocluse.LiquidSnow.Cryptography.Classical.Ciphers;
using Ocluse.LiquidSnow.Extensions;
using System.Text;

namespace Ocluse.LiquidSnow.Cryptography.Classical
{
    /// <summary>
    /// Represents a collection of characters that form an alphabet.
    /// </summary>
    public class Alphabet : IList<char>
    {
        #region Predefined Alphabets
        /// <summary>
        /// Gets an <see cref="Alphabet"/> that contains all the printable ASCII characters, from 0x20 to 0x7e
        /// </summary>
        public static Alphabet ASCII
        {
            get
            {
                var alpha = new Alphabet();
                for (int i = 0x20; i <= 0x7e; i++)
                {
                    char c = Convert.ToChar(i);
                    alpha.Add(c);
                }

                return alpha;
            }
        }

        /// <summary>
        /// Gets an <see cref="Alphabet"/> that contains all the printable ASCII characters, plus 5 more characters (âéìöú).
        /// </summary>
        /// <remarks>
        /// The count of characters in this alphabet is exactly 100, making it suitable for use with algorithms such as <see cref="Playfair"/>, which use a square grid. 
        /// </remarks>
        public static Alphabet ASCIIPerfect
        {
            get
            {
                var result = new Alphabet(ASCII.ToString());
                result.AddAll("âéìöú");
                result.AutoDimensions();
                return result;
            }
        }

        /// <summary>
        /// Gets an <see cref="Alphabet"/> that contains the 26 upper case letters of the English alphabet
        /// </summary>
        public static Alphabet EnglishUppercase
        {
            get => new Alphabet("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }

        /// <summary>
        /// Gets an <see cref="Alphabet"/> that contains the 26 lower case letters of the English alphabet
        /// </summary>
        public static Alphabet EnglishLowercase
        {
            get => new Alphabet("abcdefghijklmnopqrstuvwxyz");
        }

        /// <summary>
        /// Gets an <see cref="Alphabet"/> that contains both lower and upper case letters, plus numbers.
        /// </summary>
        public static Alphabet AlphaNumeric
        {
            get => new Alphabet("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
        }

        /// <summary>
        /// Gets an <see cref="Alphabet"/> that contains all printable characters in the Unicode character set.
        /// </summary>
        /// <remarks>
        /// Some characters may appear differently or as unknown when the font used to represent them does not contain the character.
        /// </remarks>
        public static Alphabet LargeAlphabet
        {
            get
            {
                var alpha = new Alphabet();

                for (int i = char.MinValue; i <= char.MaxValue; i++)
                {
                    char c = Convert.ToChar(i);

                    if (char.IsControl(c)) continue;
                    alpha.Add(c);
                }
                alpha.AutoDimensions();
                return alpha;
            }
        }

        /// <summary>
        /// Gets an <see cref="Alphabet"/> constructed of the character set the original IBM PC.
        /// </summary>
        public static Alphabet CodePage437
        {
            get
            {
                var alpha = new Alphabet();
                CodePagesEncodingProvider.Instance.GetEncoding(437);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                Encoding cp437 = Encoding.GetEncoding(437);
                byte[] source = new byte[1];

                for (byte i = 0x20; i < 0xFF; i++)
                {
                    source[0] = i;
                    alpha.Add(cp437.GetString(source)[0]);
                }

                var characterSet = alpha.ToString();
                return new Alphabet(characterSet);
            }
        }

        #endregion

        #region Privates
        private readonly List<char> _items;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new empty <see cref="Alphabet"/>.
        /// </summary>
        public Alphabet()
        {
            _items = [];
        }

        /// <summary>
        /// Creates a new <see cref="Alphabet"/> from the characters in the string.
        /// </summary>
        /// <param name="characters"></param>
        public Alphabet(string characters)
        {
            _items = [];

            AddAll(characters);
        }

        /// <summary>
        /// Creates a new <see cref="Alphabet"/> from the characters in the collection
        /// </summary>
        /// <param name="characters"></param>
        public Alphabet(IEnumerable<char> characters)
        {
            _items = [];
            AddAll(characters);

        }

        #endregion

        #region Properties
        /// <summary>
        /// The dimensions of the <see cref="Alphabet"/>.
        /// </summary>
        /// <remarks>
        /// This represents the rows and columns of the <see cref="Alphabet"/>, and is typically used with grid based ciphers search as <see cref="Playfair"/>.
        /// </remarks>
        public Dimensions Dimensions { get; set; }


        /// <summary>
        /// Gets a value indicating whether the count of characters in the <see cref="Alphabet"/> is a perfect square.
        /// </summary>
        /// <remarks>
        /// This is useful for checking whether square dimensions are possible.
        /// </remarks>
        public bool IsPerfectSquare => ((double)_items.Count).IsPerfectSquare();

        #endregion

        #region Public Methods

        /// <summary>
        /// Rotates the <see cref="Alphabet"/>, wrapping the end or first characters as necessary.
        /// </summary>
        /// <param name="offset">The offset to apply for the rotation</param>
        public void Rotate(int offset)
        {
            _items.Rotate(offset);
        }

        /// <summary>
        /// Shuffles the <see cref="Alphabet"/>, reordering the characters randomly.
        /// </summary>
        public void Shuffle()
        {
            _items.Shuffle();
        }

        /// <summary>
        /// Moves the character to the new index
        /// </summary>
        /// <param name="item">The character to move</param>
        /// <param name="newIndex">The new index</param>
        public void Move(char item, int newIndex)
        {
            _items.Move(item, newIndex);
        }

        ///<summary>
        /// Attempts to automatically resize the <see cref="Dimensions"/> to be as squarish as possible.
        ///</summary>
        /// <remarks>
        /// Recreates the <see cref="Dimensions"/> automatically depending on the item count.
        /// For example an alphabet with 25 characters gets a 5x5 dimensions while that with
        /// 12 characters gets a 4x3 dimension.
        /// </remarks>
        public void AutoDimensions()
        {
            //Prioritize Sqrt:
            if (IsPerfectSquare)
            {
                var root = (int)Math.Sqrt(Count);
                Dimensions = new Dimensions(root, root);
                return;
            }

            var factor = (int)((ulong)Count).MaxFactor();

            Dimensions = new Dimensions(factor, Count / factor);
        }

        /// <summary>
        /// Returns the dimensional position of a character in the <see cref="Alphabet"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Dimensions DimensionsOf(char item)
        {
            var index = IndexOf(item);

            if (index == -1)
            {
                throw new ArgumentOutOfRangeException(nameof(item), item, "The character does not exist in the alphabet");
            }
            var x = index % Dimensions.X;
            var y = index / Dimensions.X;

            return new Dimensions(x, y);
        }

        /// <summary>
        /// Returns the logical index of the character at the specified dimension
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int GetDimensionalIndex(int x, int y)
        {
            if (x > Dimensions.X) throw new ArgumentOutOfRangeException(nameof(x), x, $"X is greater than the {nameof(Dimensions)}");
            if (y >= Dimensions.Y) throw new ArgumentOutOfRangeException(nameof(y), y, $"y greater than the {nameof(Dimensions)}");

            return (Dimensions.X * y) + x;
        }

        /// <summary>
        /// Starting from the index of the provided character, this method offsets
        /// that index by the number of steps and finds the character at the new index,
        /// wrapping around the <see cref="Alphabet"/> as necessary. Useful for example in shifting for <see cref="Caesar"/>.
        /// </summary>
        /// <param name="item">The character to start at</param>
        /// <param name="steps">The number of steps to apply</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public char WrapChar(char item, int steps)
        {
            var index = IndexOf(item);

            if (index == -1) throw new ArgumentOutOfRangeException(nameof(item), item, "The character does not exist in the alphabet");

            index += steps;
            while (index >= Count) index -= Count;

            while (index < 0) index += Count;

            return this[index];
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Retrieves the character at the point represented by the dimension.
        /// </summary>
        /// <param name="dimension">The dimension representing the position of the character</param>
        public char this[Dimensions dimension]
        {
            get
            {
                return this[dimension.X, dimension.Y];
            }
            set
            {
                this[dimension.X, dimension.Y] = value;
            }
        }

        /// <summary>
        /// Retrieves a character at the provided position.
        /// </summary>
        /// <param name="x">The column of the character</param>
        /// <param name="y">The row of the character</param>
        public char this[int x, int y]
        {
            get
            {
                var index = GetDimensionalIndex(x, y);
                return this[index];
            }
            set
            {
                var index = GetDimensionalIndex(x, y);
                this[index] = value;
            }
        }

        #endregion

        #region Object Overrides
        /// <summary>
        /// Returns a string of all the characters in the <see cref="Alphabet"/>.
        /// </summary>
        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var c in this)
            {
                builder.Append(c);
            }

            return builder.ToString();
        }
        #endregion

        #region IList
        /// <summary>
        /// Adds all the characters in the collection to the alphabet,
        /// returns the number of characters added to the alphabet.
        /// </summary>
        public int AddAll(IEnumerable<char> characters)
        {
            var added = 0;
            foreach (var c in characters)
            {
                if (Add(c)) added++;
            }

            return added;
        }

        /// <summary>
        /// Adds all the characters in the string to the <see cref="Alphabet"/>.
        /// </summary>
        /// <returns>
        /// The count of characters successfully added.
        /// </returns>
        public int AddAll(string characters)
        {
            return AddAll(characters.ToCharArray());
        }

        /// <summary>
        /// Removes all the characters in the string from the <see cref="Alphabet"/>.
        /// </summary>
        /// <returns>
        /// The count of characters successfully removed.
        /// </returns>
        public int RemoveAll(string characters)
        {
            return RemoveAll(characters.ToCharArray());
        }

        /// <summary>
        /// Removes all the characters in the collection from the <see cref="Alphabet"/>.
        /// </summary>
        /// <returns>
        /// The count of characters successfully removed.
        /// </returns>
        public int RemoveAll(IEnumerable<char> characters)
        {
            var removed = 0;
            foreach (var c in characters)
            {
                if (Remove(c)) removed++;
            }
            return removed;
        }

        /// <summary>
        /// Gets the number of characters in the <see cref="Alphabet"/>.
        /// </summary>
        public int Count => _items.Count;

        ///<inheritdoc/>
        ///<remarks>This is always false for the <see cref="Alphabet"/></remarks>
        bool ICollection<char>.IsReadOnly => false;

        /// <summary>
        /// Gets or sets a character at the provided index
        /// </summary>
        /// <param name="index">The index of the character</param>
        public char this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        ///<inheritdoc/>
        public IEnumerator<char> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Returns the index of the character, or -1 if not found in the alphabet
        /// </summary>
        public int IndexOf(char item)
        {
            return _items.IndexOf(item);
        }

        /// <summary>
        /// Inserts a character at the specified position, if the character
        /// exists in the alphabet, nothing happens.
        /// </summary>
        /// <param name="index">The index to insert the character</param>
        /// <param name="item">The character to insert</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void Insert(int index, char item)
        {
            if (_items.Contains(item)) return;
            _items.Insert(index, item);
        }

        /// <summary>
        /// Removes the character at the specified index in the <see cref="Alphabet"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        /// <summary>
        /// Adds the character to the <see cref="Alphabet"/>.
        /// </summary>
        /// <returns>
        /// True if the character was added, but false if the character already exists in the <see cref="Alphabet"/>.
        /// </returns>
        public bool Add(char item)
        {
            if (Contains(item)) return false;
            _items.Add(item);
            return true;
        }

        ///<inheritdoc/>
        void ICollection<char>.Add(char item)
        {
            if (Contains(item)) return;
            _items.Add(item);
            return;
        }

        /// <summary>
        /// Clear all characters from the <see cref="Alphabet"/>.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }

        /// <summary>
        /// Checks if the <see cref="Alphabet"/> contains the character.
        /// </summary>
        /// <returns>
        /// True if the character exists, otherwise false.
        /// </returns>
        public bool Contains(char item)
        {
            return _items.Contains(item);
        }

        /// <summary>
        /// Copies the characters in the <see cref="Alphabet"/> to the provided character array.
        /// </summary>
        /// <param name="array">The array to copy the characters to.</param>
        /// <param name="arrayIndex">The index to start copying the characters.</param>
        public void CopyTo(char[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes a character from the <see cref="Alphabet"/>.
        /// </summary>
        /// <returns>
        /// True if the character was removed, otherwise false.
        /// </returns>
        public bool Remove(char item)
        {
            return _items.Remove(item);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion
    }
}
