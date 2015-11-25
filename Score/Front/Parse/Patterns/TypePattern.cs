namespace Score.Front.Parse.Patterns
{
    using Types;

    internal sealed class TypePattern : BasePattern
    {
        public TypeInfo type;
        public TuplePattern pattern;

        public TypePattern(TypeInfo type, TuplePattern pattern)
        {
            this.type = type;
            this.pattern = pattern;
        }
    }
}
