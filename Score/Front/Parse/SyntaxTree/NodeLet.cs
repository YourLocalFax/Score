namespace Score.Front.Parse.SyntaxTree
{
    using Data;

    internal sealed class NodeLet : Node
    {
        // TODO(kai): Just for temps

        public readonly Parameter binding;
        public readonly NodeExpr value;

        internal override Span Span => binding.name.Span;

        public NodeLet(Parameter binding, NodeExpr value)
        {
            this.binding = binding;
            this.value = value;
        }

        // TODO(kai): welp

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
