using System;
using System.Collections.Generic;

namespace Score.Front.Parse.SyntaxTree
{
    internal sealed class NodeIf : NodeExpr
    {
        public sealed class IfBlock
        {
            public readonly NodeExpr condition;
            public readonly List<Node> body;

            // TODO(kai): Do this better
            public Span Span => condition.Span;

            public IfBlock(NodeExpr condition, List<Node> body)
            {
                this.condition = condition;
                this.body = body;
            }
        }

        public List<IfBlock> conditions = new List<IfBlock>();
        public List<Node> fail = new List<Node>();

        // TODO(kai): Do this better
        public override Span Span => conditions[0].Span;

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
