using Source;
using Ty;

namespace SyntaxTree.Data
{
    public sealed class Parameter
    {
        public readonly Spanned<string> name;
        public Spanned<TyRef> spTy;

        public string Name => name.value;
        public Span TySpan => spTy.span;
        public TyRef Ty => spTy.value;

        public bool HasName => name != null;
        public bool InferTy => Ty is InferTyRef;
        public bool HasTy => !InferTy;

        // TODO(kai): default values

        public Parameter(Spanned<string> name, Spanned<TyRef> spTy)
        {
            this.spTy = spTy;
            this.name = name;
        }
    }
}
