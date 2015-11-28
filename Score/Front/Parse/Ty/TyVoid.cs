namespace Score.Front.Parse.Ty
{
    internal sealed class TyVoid : TyPrimitive
    {
        public TyVoid(Span span) : base(span) { }

        public override string ToString() => "()";
    }
}
