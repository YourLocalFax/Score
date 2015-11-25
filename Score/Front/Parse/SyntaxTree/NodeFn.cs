using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse.SyntaxTree
{
    internal sealed class NodeFn : Node
    {
        public NodeFnDecl decl;
        public Location end;

        public bool hasEq = false;
        public List<Node> body;

        internal override Span Span => decl.start + end;

        public NodeFn(NodeFnDecl decl)
        {
            this.decl = decl;
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
