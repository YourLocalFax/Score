using System.Collections.Generic;

using Lex;
using Source;

namespace Ast
{
    public sealed class NodeTuple : NodeExpr
    {
        public readonly Token spLParen, spRParen;
        public List<NodeExpr> values;

        /// <summary>
        /// Location information for the tokens that this node was created from.
        /// </summary>
        public override Span Span => new Span(spLParen.span.fileName, spLParen.span.start, spRParen.span.end);

        public NodeTuple(Token spLParen, Token spRParen, List<NodeExpr> values)
        {
            this.spLParen = spLParen;
            this.spRParen = spRParen;
            this.values = values;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
