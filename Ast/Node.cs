using Source;

namespace Ast
{
    /// <summary>
    /// The base class for every node of the Abstract Syntax Tree.
    /// </summary>
    public abstract class Node
    {
        public virtual Location Start => Span.start;

        public abstract Span Span { get; }

        public abstract void Accept(IAstVisitor visitor);
    }
}
