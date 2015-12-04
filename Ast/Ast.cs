using System.Collections.Generic;

using Source;

namespace Ast
{
    public sealed class SyntaxTree : Node
    {
        public readonly List<Node> children = new List<Node>();

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
