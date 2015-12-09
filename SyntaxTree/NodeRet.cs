using Lex;
using Source;

namespace SyntaxTree
{
    public sealed class NodeRet : Node
    {
        public readonly Token ret;
        public readonly NodeExpr value;

        public bool IsVoidRet => value == null;

        public override Span Span => ret.span;

        public NodeRet(Token ret, NodeExpr value)
        {
            this.ret = ret;
            this.value = value;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
