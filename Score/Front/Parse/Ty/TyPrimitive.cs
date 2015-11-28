namespace Score.Front.Parse.Ty
{
    internal abstract class TyPrimitive : TyVariant
    {
        private readonly Span span;
        public override Span Span => span;

        public TyPrimitive(Span span)
        {
            this.span = span;
        }
    }
}
