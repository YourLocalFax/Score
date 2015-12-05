using System.Collections.Generic;

using Source;

namespace Ast
{
    public sealed class SyntaxTree : Node
    {
        public readonly List<Node> children = new List<Node>();

        public override Span Span => children.Count == 0 ? default(Span) :
            new Span(children[0].Span.fileName, children[0].Span.start, children[children.Count - 1].Span.end);

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
