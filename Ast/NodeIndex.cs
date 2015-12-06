using Lex;
using Source;

namespace SyntaxTree
{
    public sealed class NodeIndex : NodeExpr
    {
        public NodeExpr target;
        public Token spDot;
        public NodeId index;

        public NodeExpr Target => target;
        public string Index => index.Image;

        public override Span Span => new Span(target.Span.fileName, target.Span.start, index.Span.end);

        public NodeIndex(NodeExpr target, Token spDot, NodeId index)
        {
            this.target = target;
            this.spDot = spDot;
            this.index = index;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
