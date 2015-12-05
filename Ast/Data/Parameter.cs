using Source;
using Ty;

namespace Ast.Data
{
    public sealed class Parameter
    {
        public readonly Spanned<string> name;
        public Spanned<TyRef> spTy;

        public Span TySpan => spTy.span;
        public TyRef Ty => spTy.value;

        public bool HasName => name != null;
        public bool HasTy => spTy != null;

        // TODO(kai): default values

        public Parameter(Spanned<string> name, Spanned<TyRef> spTy)
        {
            this.spTy = spTy;
            this.name = name;
        }
    }
}
