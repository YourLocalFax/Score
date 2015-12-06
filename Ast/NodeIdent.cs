using Lex;
using Source;

namespace Ast
{
    /// <summary>
    /// Represents an identifier.
    /// </summary>
    public sealed class NodeId : NodeExpr
    {
        private readonly Token spToken;

        /// <summary>
        /// The string representation of this identifier.
        /// </summary>
        public string Image => Token.Image;

        /// <summary>
        /// The token that this node was created from.
        /// </summary>
        public Token Token => spToken;
        /// <summary>
        /// Location information for the token that this node was created from.
        /// </summary>
        public override Span Span => spToken.span;

        public NodeId(Token token)
        {
            spToken = token;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
