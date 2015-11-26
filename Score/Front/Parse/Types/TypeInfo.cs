using System.Linq;
using System.Text;

namespace Score.Front.Parse.Types
{
    internal abstract class TypeInfo
    {
        public static readonly TupleTypeInfo VOID = new TupleTypeInfo();
        public static readonly PathTypeInfo INFER = new PathTypeInfo(null);
    }

    internal class PathTypeInfo : TypeInfo
    {
        public readonly IdentPath path;

        public PathTypeInfo(IdentPath path)
        {
            this.path = path;
        }

        public override string ToString()
        {
            return path.ToString();
        }
    }

    internal sealed class PointerTypeInfo : TypeInfo
    {
        public TypeInfo type;
        public bool isMut;

        public PointerTypeInfo(TypeInfo type, bool isMut)
        {
            this.type = type;
            this.isMut = isMut;
        }

        public override string ToString()
        {
            return string.Format("^{0}{1}", isMut ? "mut " : "", type.ToString());
        }
    }

    internal sealed class ReferenceTypeInfo : TypeInfo
    {
        public TypeInfo type;
        public bool isMut;

        public ReferenceTypeInfo(TypeInfo type, bool isMut)
        {
            this.type = type;
            this.isMut = isMut;
        }

        public override string ToString()
        {
            return string.Format("&{0}{1}", isMut ? "mut " : "", type.ToString());
        }
    }

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

    internal sealed class ArrayTypeInfo : TypeInfo
    {
        public readonly TypeInfo contained;

        public ArrayTypeInfo(TypeInfo contained)
        {
            this.contained = contained;
        }

        public override string ToString()
        {
            return "[" + contained.ToString() + "]";
        }
    }
}
