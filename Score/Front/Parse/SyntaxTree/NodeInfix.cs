namespace Score.Front.Parse.SyntaxTree
{
    using Lex;

    internal sealed class NodeInfix : NodeExpr
    {
        public NodeExpr left, right;
        public TokenOp op;

        public override Span Span => left.Span.Start + right.Span.End;

        public NodeInfix(NodeExpr left, NodeExpr right, TokenOp op)
        {
            this.left = left;
            this.right = right;
            this.op = op;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
