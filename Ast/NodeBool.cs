using Lex;
using Source;

namespace Ast
{
    /// <summary>
    /// Represents a bool literal.
    /// </summary>
    public sealed class NodeBool : NodeExpr
    {
        private readonly Token spToken;

        /// <summary>
        /// The value of this literal.
        /// </summary>
        public bool Value { get; private set; }

        /// <summary>
        /// The token that this node was created from.
        /// </summary>
        public Token Token => spToken;
        /// <summary>
        /// Location information for the token that this node was created from.
        /// </summary>
        public override Span Span => spToken.span;

        public NodeBool(Token token)
        {
            spToken = token;
            Value = Token.Image == "true";
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
