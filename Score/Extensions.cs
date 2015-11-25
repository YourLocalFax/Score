using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Score
{
    internal static class Extensions
    {
        public static string Repeat(this string s, int count)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < count; i++)
                builder.Append(s);
            return builder.ToString();
        }

        public static V GetOrCreate<K, V>(this Dictionary<K, V> dict, K key, Func<V> valueFunc)
        {
            V value;
            if (!dict.TryGetValue(key, out value))
                dict[key] = value = valueFunc();
            return value;
        }

        public static void Resize<T>(this List<T> list, int size)
        {
            int cur = list.Count;
            if (size < cur)
                list.RemoveRange(size, cur - size);
            else if (size > cur)
            {
                if (size > list.Capacity)
                    list.Capacity = size;
                list.AddRange(Enumerable.Repeat(default(T), size - cur));
            }
        }
    }
}
