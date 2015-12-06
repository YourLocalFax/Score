using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Util
{
    public static class Extensions
    {
        public static string Repeat(this string s, int count)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < count; i++)
                builder.Append(s);
            return builder.ToString();
        }

        public static void ForEach<T>(this T[] arr, Action<T> action)
        {
            foreach (var t in arr)
                action(t);
        }

        public static TResult[] Map<T, TResult>(this T[] arr, Func<T, TResult> func)
        {
            var result = new TResult[arr.Length];
            for (int i = 0, len = arr.Length; i < len; i++)
                result[i] = func(arr[i]);
            return result;
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
