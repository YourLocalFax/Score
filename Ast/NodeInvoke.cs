using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse.SyntaxTree
{
    internal sealed class NodeInvoke : NodeExpr
    {
        public NodeExpr target;
        public List<NodeExpr> args;

        public override Span Span => target.Span.Start + args[args.Count - 1].Span.End;

        public NodeInvoke(NodeExpr target, List<NodeExpr> args)
        {
            this.target = target;
            this.args = args;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
