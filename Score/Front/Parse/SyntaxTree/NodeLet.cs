using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse.SyntaxTree
{
    internal sealed class NodeLet : Node
    {
        internal override Span Span => new Span();

        // TODO(kai): welp

        public override void Accept(IAstVisitor visitor)
        {
            //visitor.Visit(this);
        }
    }
}
