namespace Score.Front.Parse.SyntaxTree
{
    using Lex;

    internal sealed class NodeSuffix : NodeExpr
    {
        public NodeExpr target;
        public TokenOp op;

        public override Span Span => target.Span.Start + op.span.End;

        public NodeSuffix(NodeExpr target, TokenOp op)
        {
            this.target = target;
            this.op = op;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
