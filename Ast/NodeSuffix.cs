using Lex;
using Source;

namespace Ast
{
    public sealed class NodeSuffix : NodeExpr
    {
        public NodeExpr target;
        public Token op;

        public NodeExpr Target => target;
        public Token Op => op;

        public override Span Span => new Span(target.Span.fileName, target.Span.start, op.span.end);

        public NodeSuffix(NodeExpr target, Token op)
        {
            this.target = target;
            this.op = op;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
