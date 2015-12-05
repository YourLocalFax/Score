using System.Collections.Generic;

using Lex;
using Source;

namespace Ast
{
    public sealed class NodeTuple : NodeExpr
    {
        public readonly Spanned<Token> spLParen, spRParen;
        public List<Spanned<NodeExpr>> values;

        /// <summary>
        /// Location information for the tokens that this node was created from.
        /// </summary>
        public override Span Span => new Span(spLParen.span.fileName, spLParen.span.start, spRParen.span.end);

        public NodeTuple(Spanned<Token> spLParen, Spanned<Token> spRParen, List<Spanned<NodeExpr>> values)
        {
            this.spLParen = spLParen;
            this.spRParen = spRParen;
            this.values = values;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
