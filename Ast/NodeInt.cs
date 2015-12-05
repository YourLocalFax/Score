using Lex;
using Source;

namespace Ast
{
    public sealed class NodeInt : NodeExpr
    {
        public readonly Spanned<Token> spToken;

        /// <summary>
        /// The value of this literal.
        /// </summary>
        public ulong Value { get; private set; }

        /// <summary>
        /// The token that this node was created from.
        /// </summary>
        public Token Token => spToken.value;
        /// <summary>
        /// Location information for the token that this node was created from.
        /// </summary>
        public override Span Span => spToken.span;

        public NodeInt(Spanned<Token> spToken)
        {
            this.spToken = spToken;
            Value = spToken.value.IntegerValue;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
