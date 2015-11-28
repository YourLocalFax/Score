using System.Collections.Generic;

namespace Score.Front.Parse.SyntaxTree
{
    internal sealed class NodeTuple : NodeExpr
    {
        public Location start, end;
        public List<NodeExpr> values;

        internal override Span Span => start + end;

        public NodeTuple(List<NodeExpr> values, Location start, Location end)
        {
            this.values = values;
            this.start = start;
            this.end = end;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
