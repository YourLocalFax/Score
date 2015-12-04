namespace Ast
{
    /// <summary>
    /// The base class for every node of the Abstract Syntax Tree.
    /// </summary>
    public abstract class Node
    {
        public abstract void Accept(IAstVisitor visitor);
    }
}
