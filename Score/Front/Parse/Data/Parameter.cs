namespace Score.Front.Parse.Data
{
    using Ty;

    internal sealed class Parameter
    {
        public readonly Name name;
        // TODO(kai): use spanned types
        public Spanned<TyRef> spTy;

        public Span TySpan => spTy.span;
        public TyRef Ty => spTy?.value;

        public bool IsNamed => name != null;
        public bool IsTyd => spTy != null;

        // TODO(kai): default values

        public Parameter(Name name, Spanned<TyRef> spTy)
        {
            this.spTy = spTy;
            this.name = name;
        }
    }
}
