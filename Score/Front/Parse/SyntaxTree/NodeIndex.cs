using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse.SyntaxTree
{
    using Lex;

    internal sealed class NodeIndex : NodeExpr
    {
        public NodeExpr target;
        public NodeId index;

        internal override Span Span => target.Span.Start + index.Span.End;

        public NodeIndex(NodeExpr target, NodeId index)
        {
            this.target = target;
            this.index = index;
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
