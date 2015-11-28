namespace Score.Front.Parse.Ty
{
    internal sealed class TyBool : TyPrimitive
    {
        public TyBool(Span span) : base(span) { }

        public override string ToString() => "bool";
    }
}
