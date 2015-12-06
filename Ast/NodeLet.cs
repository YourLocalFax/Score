using Source;

namespace SyntaxTree
{
    using Data;

    public sealed class NodeLet : Node
    {
        // TODO(kai): Just for temps

        public readonly Parameter binding;
        public readonly NodeExpr value;

        public override Span Span => binding.name.span;

        public NodeLet(Parameter binding, NodeExpr value)
        {
            this.binding = binding;
            this.value = value;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
