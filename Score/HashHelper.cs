using System.Collections.Generic;

namespace Score
{
    internal static class HashHelper
    {
        public static int GetHashCode(params object[] objects)
        {
            unchecked
            {
                int hash = 0;
                foreach (var item in objects)
                    if (item != null)
                        hash = 31 * hash + item.GetHashCode();
                return hash;
            }
        }

        public static int GetHashCode<T>(IEnumerable<T> list)
        {
            unchecked
            {
                int hash = 0;
                foreach (var item in list)
                    if (item != null)
                        hash = 31 * hash + item.GetHashCode();
                return hash;
            }
        }
    }
}
