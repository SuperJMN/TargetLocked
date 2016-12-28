namespace Identification
{
    using System;
    using System.Collections.Generic;

    public static class RxUtils
    {
        public static IEnumerable<T> WhereIndex<T>(this IEnumerable<T> collection, Func<int, bool> predicate)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var index = -1;
            using (var iter = collection.GetEnumerator())
            {
                while (iter.MoveNext())
                {
                    index++;

                    if (!predicate(index)) continue;
                    yield return iter.Current;
                }
            }
        }
    }
}