using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse.SyntaxTree
{
    using Lex;

    internal sealed class NodeId : NodeExpr
    {
        public TokenId token;

        internal override Span Span => token.span;

        public NodeId(TokenId token)
        {
            this.token = token;
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
