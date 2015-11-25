using System.Linq;
using System.Text;

namespace Score.Front.Parse.Types
{
    internal sealed class TupleTypeInfo : TypeInfo
    {
        public readonly TypeInfo[] contained;

        public TupleTypeInfo(params TypeInfo[] contained)
        {
            this.contained = contained;
        }

        public override string ToString()
        {
            var s = contained.Aggregate(new StringBuilder(), (sb, c) =>
            {
                sb.Append(c.ToString());
                sb.Append(", ");
                return sb;
            }).ToString();
            return "(" + s.Substring(0, s.Length - 2) + ")"; // TODO(kai): trim the last two chars pls
        }
    }
}
