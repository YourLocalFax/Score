using Lex;
using Source;

namespace SyntaxTree
{
    /// <summary>
    /// Represents an expression enclosed in parenthesis, but not a tuple.
    /// </summary>
    public sealed class NodeEnclosed : NodeExpr
    {
        public readonly Token lparen, rparen;
        public readonly NodeExpr expr;

        /// <summary>
        /// Location information for the tokens that this node was created from.
        /// </summary>
        public override Span Span => new Span(lparen.span.fileName, lparen.span.start, rparen.span.end);

        public NodeEnclosed(Token lparen, Token rparen, NodeExpr expr)
        {
            this.lparen = lparen;
            this.rparen = rparen;
            this.expr = expr;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
