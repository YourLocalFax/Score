using Lex;
using Source;

namespace Ast
{
    public sealed class NodeInfix : NodeExpr
    {
        public Spanned<NodeExpr> left, right;
        public Spanned<Token> op;

        public NodeExpr Left => left.value;
        public NodeExpr Right => right.value;
        public Token Op => op.value;

        public override Span Span => new Span(left.span.fileName, left.span.start, right.span.end);

        public NodeInfix(Spanned<NodeExpr> left, Spanned<NodeExpr> right, Spanned<Token> op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
