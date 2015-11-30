using System;
using System.Collections.Generic;

namespace Score.Middle.Symbols
{
    using static SymbolTable;

    internal sealed class SymbolTableWalker
    {
        private sealed class Node
        {
            private readonly Scope anchor;
            private readonly List<Node> children = new List<Node>();
            private int index = -1;

            public bool IsOver => index >= children.Count;

            public Node(Scope anchor)
            {
                this.anchor = anchor;
                for (int i = 0; i < anchor.ScopeCount; i++)
                    children.Add(new Node(anchor.GetScope(i)));
            }

            // NOTE(kai): SHOULD NOT BE CALLED IF IsOver RETURNED TRUE
            public Scope Get() => index == -1 ? anchor : IsOver ? null : children[index].Get();

            public Node Step()
            {
                if (index >= 0)
                {
                    if (!children[index].IsOver)
                        children[index].Step();
                    else index++;
                }
                else index++;
                return this;
            }
        }

        public readonly SymbolTable symbols;
        private readonly Node root;

        public Scope Current => root.Get();

        public SymbolTableWalker(SymbolTable symbols)
        {
            this.symbols = symbols;
            root = new Node(symbols.global);
        }

        public void Step()
        {
            /*
            Console.WriteLine("Preparint to step, you have: ");
            foreach (var key in Current.symbols.Keys)
                Console.WriteLine(key);
            //*/
            root.Step();
            /*
            Console.WriteLine("Finished stepping, you have: ");
            foreach (var key in Current.symbols.Keys)
                Console.WriteLine(key);
            Console.WriteLine();
            //*/
        }
    }
}
