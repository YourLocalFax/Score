namespace Score.Front.Parse.Types
{
    internal abstract class TypeInfo
    {
        public static readonly TupleTypeInfo VOID = new TupleTypeInfo();
        public static readonly PathTypeInfo INFER = new PathTypeInfo(null);
    }
}
