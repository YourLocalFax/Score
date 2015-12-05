using Lex;
using Source;

namespace Ast
{
    /// <summary>
    /// Represents an expression enclosed in parenthesis, but not a tuple.
    /// </summary>
    public sealed class NodeEnclosed : NodeExpr
    {
        public readonly Spanned<Token> spLParen, spRParen;
        public readonly Spanned<NodeExpr> spExpr;

        /// <summary>
        /// Location information for the tokens that this node was created from.
        /// </summary>
        public override Span Span => new Span(spLParen.span.fileName, spLParen.span.start, spRParen.span.end);

        public NodeEnclosed(Spanned<Token> spLParen, Spanned<Token> spRParen, Spanned<NodeExpr> spExpr)
        {
            this.spLParen = spLParen;
            this.spRParen = spRParen;
            this.spExpr = spExpr;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
