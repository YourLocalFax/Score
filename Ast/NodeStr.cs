using Lex;
using Source;

namespace Ast
{
    public sealed class NodeStr : NodeExpr
    {
        public readonly Spanned<Token> spToken;

        /// <summary>
        /// The value of this literal.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Whether or not this string is verbatim.
        /// </summary>
        public bool Verbatim { get; private set; }
        /// <summary>
        /// Whether or not this string is a c-string.
        /// </summary>
        public bool CStr { get; private set; }

        /// <summary>
        /// The token that this node was created from.
        /// </summary>
        public Token Token => spToken.value;
        /// <summary>
        /// Location information for the token that this node was created from.
        /// </summary>
        public override Span Span => spToken.span;

        public NodeStr(Spanned<Token> spToken)
        {
            this.spToken = spToken;
            Value = spToken.value.Image;
            Verbatim = spToken.value.StrVerbatim;
            CStr = spToken.value.StrC;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
