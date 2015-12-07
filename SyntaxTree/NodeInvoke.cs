using System.Collections.Generic;

using Source;

namespace SyntaxTree
{
    public sealed class NodeInvoke : NodeExpr
    {
        public NodeExpr target;
        public List<NodeExpr> args;

        public override Span Span => new Span(target.Span.fileName, target.Span.start, args[args.Count - 1].Span.end);

        public NodeInvoke(NodeExpr target, List<NodeExpr> args)
        {
            this.target = target;
            this.args = args;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
