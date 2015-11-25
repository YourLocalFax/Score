using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse.SyntaxTree
{
    using Lex;

    internal sealed class NodeInt : NodeExpr
    {
        public TokenInt token;

        internal override Span Span => token.span;

        public NodeInt(TokenInt token)
        {
            this.token = token;
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
