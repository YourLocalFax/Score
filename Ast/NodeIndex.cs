using Lex;
using Source;

namespace Ast
{
    public sealed class NodeIndex : NodeExpr
    {
        public Spanned<NodeExpr> target;
        public Spanned<Token> spDot;
        public NodeId index;

        public NodeIndex(Spanned<NodeExpr> target, Spanned<Token> spDot, NodeId index)
        {
            this.target = target;
            this.spDot = spDot;
            this.index = index;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
