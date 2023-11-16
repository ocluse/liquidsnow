namespace Ocluse.LiquidSnow.Extensions
{
    /// <summary>
    /// Extension methods for the System.Collections.ObjectModel namespace.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Moves an item at the specified index to the new index
        /// </summary>
        /// <param name="list">The list to perform the operation on</param>
        /// <param name="oldIndex">The current index of the item</param>
        /// <param name="newIndex">The new index of the item</param>
        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {

            var item = list[oldIndex];

            list.RemoveAt(oldIndex);

            if (newIndex == list.Count)
            {
                list.Add(item);
            }
            else
            {
                list.Insert(newIndex, item);
            }
        }

        /// <summary>
        /// Checks for a duplicate item in a list
        /// </summary>
        public static bool HasDuplicate<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            HashSet<T> checkBuffer;
            if (comparer == null)
            {
                checkBuffer = [];
            }
            else
            {
                checkBuffer = new HashSet<T>(comparer);
            }

            foreach (var t in source)
            {
                if (checkBuffer.Add(t))
                {
                    continue;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the unique duplicate items.
        /// </summary>
        public static IEnumerable<T> GetDuplicates<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer = null)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            HashSet<T> checkBuffer;

            HashSet<T> result;


            if (comparer == null)
            {
                checkBuffer = [];
                result = [];
            }
            else
            {
                checkBuffer = new HashSet<T>(comparer);
                result = new HashSet<T>(comparer);
            }

            foreach (var t in source)
            {
                if (checkBuffer.Add(t))
                {
                    continue;
                }
                else
                {
                    result.Add(t);
                }
            }

            return result;
        }

        /// <summary>
        /// Moves an item in the list to the specified index
        /// </summary>
        /// <param name="list">The list to perform the operation on</param>
        /// <param name="item">The item to move</param>
        /// <param name="newIndex">The index to move the item to</param>
        public static void Move<T>(this IList<T> list, T item, int newIndex)
        {
            var oldIndex = list.IndexOf(item);

            if (oldIndex == -1)
                throw new InvalidOperationException("Item not found in list");

            list.Move(oldIndex, newIndex);
        }

        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var random = new Random();
                var r = random.Next(i, count);
                (ts[r], ts[i]) = (ts[i], ts[r]);
            }
        }

        /// <inheritdoc cref="Shuffle{T}(IList{T})"/>
        public static void Shuffle<T>(this IList<T> ts, int seed)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var random = new Random(seed);
                var r = random.Next(i, count);
                (ts[r], ts[i]) = (ts[i], ts[r]);
            }
        }

        /// <summary>
        /// Rotates the items on a list i.e offsets the positions of the items, wrapping where necessary
        /// </summary>
        /// <param name="list">The list to perform the operation on</param>
        /// <param name="offset">How much to shift the items</param>
        /// <returns></returns>
        public static void Rotate<T>(this IList<T> list, int offset)
        {
            if (offset >= 0)
            {
                for (; offset > 0; offset--)
                {
                    T first = list[0];
                    list.RemoveAt(0);
                    list.Add(first);
                }

            }
            else
            {
                for (; offset <= 0; offset++)
                {
                    var index = list.Count - 1;
                    T last = list[index];
                    list.RemoveAt(index);
                    list.Insert(0, last);
                }
            }
        }

        /// <summary>
        /// Returns a random item from the sequence. If the sequence is empty, an exception is thrown.
        /// </summary>
        public static T Random<T>(this IEnumerable<T> source, Func<T, bool> expression)
        {
            return source.Where(expression).Random();
        }

        /// <summary>
        /// Returns a random item from the sequence. If the sequence is empty, an exception is thrown.
        /// </summary>
        public static T Random<T>(this IEnumerable<T> source)
        {
            List<T> copy = new(source);
            if (copy.Count == 0) throw new InvalidOperationException("Sequence contains no elements");
            if (copy.Count > 1)
            {
                copy.Shuffle();
            }
            return copy.First();
        }

        /// <summary>
        /// Adds a range of items to the collection
        /// </summary>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            if (items == null) return;
            foreach (var i in items)
            {
                collection.Add(i);
            }
        }


        /// <summary>
        /// Removes all the items provided from the collection, returning the number of items removed
        /// </summary>
        public static int RemoveAll<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            int removed = 0;
            foreach (var i in items)
            {
                if (collection.Remove(i))
                {
                    removed++;
                }
            }

            return removed;
        }

        /// <summary>
        /// Removes all the items from the collection that match the predicate,
        /// returning the number of items removed
        /// </summary>
        public static int RemoveAll<T>(this ICollection<T> collection, Func<T, bool> predicate)
        {
            var removable = collection.Where(predicate).ToList();
            int removed = 0;

            foreach (var itemToRemove in removable)
            {
                if (collection.Remove(itemToRemove))
                {
                    removed++;
                }
            }

            return removed;
        }

        /// <summary>
        /// Sorts the collection.
        /// </summary>
        /// <typeparam name="T">The type of the elements of the collection.</typeparam>
        /// <param name="collection">The collection to sort.</param>
        /// <param name="comparison">The comparison used for sorting.</param>
        public static void Sort<T>(this IList<T> collection, Comparison<T>? comparison = null)
        {
            var sortableList = new List<T>(collection);
            if (comparison == null)
                sortableList.Sort();
            else
                sortableList.Sort(comparison);

            for (var i = 0; i < sortableList.Count; i++)
            {
                var oldIndex = collection.IndexOf(sortableList[i]);
                var newIndex = i;
                if (oldIndex != newIndex)
                    collection.Move(oldIndex, newIndex);
            }
        }


    }
}
