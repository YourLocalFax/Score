﻿using Lex;
using Source;

namespace SyntaxTree
{
    public sealed class NodeInt : NodeExpr
    {
        public readonly Token spToken;

        /// <summary>
        /// The value of this literal.
        /// </summary>
        public ulong Value { get; private set; }

        /// <summary>
        /// The token that this node was created from.
        /// </summary>
        public Token Token => spToken;
        /// <summary>
        /// Location information for the token that this node was created from.
        /// </summary>
        public override Span Span => spToken.span;

        public NodeInt(Token spToken)
        {
            this.spToken = spToken;
            Value = spToken.IntegerValue;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
