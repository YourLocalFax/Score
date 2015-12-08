using System.Collections.Generic;

using Source;

namespace SyntaxTree
{
    public sealed class NodeInvoke : NodeExpr
    {
        public Spanned<string> spName;
        public List<NodeExpr> args;

        public string TargetName => spName.value;

        public override Span Span => new Span(spName.span.fileName, spName.span.start, args[args.Count - 1].Span.end);

        public NodeInvoke(Spanned<string> spName, List<NodeExpr> args)
        {
            this.spName = spName;
            this.args = args;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
