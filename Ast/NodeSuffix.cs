using Lex;
using Source;

namespace Ast
{
    public sealed class NodeSuffix : NodeExpr
    {
        public Spanned<NodeExpr> target;
        public Spanned<Token> op;

        public NodeExpr Target => target.value;
        public Token Op => op.value;

        public override Span Span => new Span(target.span.fileName, target.span.start, op.span.end);

        public NodeSuffix(Spanned<NodeExpr> target, Spanned<Token> op)
        {
            this.target = target;
            this.op = op;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
