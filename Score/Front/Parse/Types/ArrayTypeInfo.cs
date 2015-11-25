namespace Score.Front.Parse.Types
{
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
