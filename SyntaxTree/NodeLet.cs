using Source;

using Ty;

namespace SyntaxTree
{
    public sealed class NodeLet : Node
    {
        // TODO(kai): Just for temps

        public readonly Spanned<string> spName;
        public Spanned<TyRef> spTy;
        public readonly NodeExpr value;

        public override Span Span => spName.span;

        public string Name => spName.value;
        public TyRef Ty => spTy.value;

        public NodeLet(Spanned<string> spName, Spanned<TyRef> spTy, NodeExpr value)
        {
            this.spName = spName;
            this.spTy = spTy;
            this.value = value;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
