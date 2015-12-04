using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse.SyntaxTree
{
    internal sealed class Ast : Node
    {
        public readonly List<Node> children = new List<Node>();

        public override Span Span => children.Count == 0 ? new Span() :
            children[0].Span.Start + children[children.Count - 1].Span.End;

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
