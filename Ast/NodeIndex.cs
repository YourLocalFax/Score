using Lex;
using Source;

namespace Ast
{
    public sealed class NodeIndex : NodeExpr
    {
        public Spanned<NodeExpr> target;
        public Spanned<Token> spDot;
        public NodeId index;

        public NodeExpr Target => target.value;
        public string Index => index.Image;

        public override Span Span => new Span(target.span.fileName, target.span.start, index.Span.end);

        public NodeIndex(Spanned<NodeExpr> target, Spanned<Token> spDot, NodeId index)
        {
            this.target = target;
            this.spDot = spDot;
            this.index = index;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
