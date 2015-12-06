using System.Text;

namespace Symbols
{
    internal static class Extensions
    {
        public static string Repeat(this string s, int count)
        {
            if (count == 0)
                return "";
            else if (count == 1)
                return s;
            var builder = new StringBuilder();
            for (int i = 0; i < count; i++)
                builder.Append(s);
            return builder.ToString();
        }
    }
}
