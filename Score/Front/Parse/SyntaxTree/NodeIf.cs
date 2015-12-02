using System.Collections.Generic;

namespace Score.Front.Parse.SyntaxTree
{
    internal sealed class NodeIf
    {
        public NodeExpr condition;
        public List<Node> pass;
        public List<Node> fail;
    }
}
