using System;
using System.Collections.Generic;
using System.Linq;

namespace des
{
    public static class EnumerableUtils
    {
        public static T Random<T>(this IEnumerable<T> collection, Random random)
        {
            if (!collection.Any() || random == null) return default(T);

            var idx = random.Next(0, collection.Count());
            return collection.ElementAt(idx);
        }

        public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, T element)
        {
            foreach (var item in collection)
            {
                if (!item.Equals(element)) yield return item;
            }
        }
    }
}
