using System.Collections.Generic;

namespace Symbols
{
    using static SymbolTable;

    public sealed class SymbolTableWalker
    {
        private sealed class Node
        {
            internal readonly Scope anchor;
            private readonly List<Node> children = new List<Node>();

            private bool inSelf = true;
            private int index = 0;

            public Node(Scope anchor)
            {
                this.anchor = anchor;
                for (int i = 0; i < anchor.ScopeCount; i++)
                    children.Add(new Node(anchor.GetScope(i)));
            }

            public Node Get() => inSelf ? this : children[index];

            public void StepIn()
            {
                if (!inSelf)
                    Get().StepIn();
                inSelf = false;
            }

            public bool StepOut()
            {
                if (inSelf)
                    return true;
                if (Get().StepOut())
                {
                    inSelf = true;
                    index++;
                }
                return false;
            }
        }

        public readonly SymbolTable symbols;
        private Node root;

        public Scope Current => root.Get().anchor;

        public SymbolTableWalker(SymbolTable symbols)
        {
            this.symbols = symbols;
            Reset();
        }

        public void Reset()
        {
            root = new Node(symbols.global);
        }

        public void StepIn() => root.StepIn();
        public void StepOut() => root.StepOut();
    }
}
