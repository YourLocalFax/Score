namespace Score.Front.Parse.SyntaxTree
{
    internal abstract class Node
    {
        public abstract Span Span { get; }

        public abstract void Accept(IAstVisitor visitor);
    }
}
