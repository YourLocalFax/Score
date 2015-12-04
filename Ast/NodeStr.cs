using System;
using System.Collections.Generic;
using System.Linq;
namespace Score.Front.Parse.SyntaxTree
{
    using Lex;

    internal sealed class NodeStr : NodeExpr
    {
        public TokenStr str;

        public override Span Span => str.span;

        public NodeStr(TokenStr str)
        {
            this.str = str;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
