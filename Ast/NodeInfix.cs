using Lex;
using Source;

namespace SyntaxTree
{
    public sealed class NodeInfix : NodeExpr
    {
        public NodeExpr left, right;
        public Token op;

        public NodeExpr Left => left;
        public NodeExpr Right => right;
        public Token Op => op;

        public override Span Span => new Span(left.Span.fileName, left.Span.start, right.Span.end);

        public NodeInfix(NodeExpr left, NodeExpr right, Token op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
