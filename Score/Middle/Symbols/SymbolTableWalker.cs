using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Middle.Symbols
{
    using static SymbolTable;

    internal sealed class SymbolTableWalker
    {
        public readonly SymbolTable symbols;

        private Scope current;
        private Stack<int> indices = new Stack<int>();

        public SymbolTableWalker(SymbolTable symbols)
        {
            this.symbols = symbols;

            current = symbols.global.GetScope(0);
            indices.Push(0);
        }

        public Scope Step()
        {
            var result = current;
            // step into this scope.
            if (current.ScopeCount > 0)
            {
                current = result.GetScope(0);
                indices.Push(0);
            }
            // No need to step into a scope, so continue inside it...
            else
            {
                var index = indices.Peek();
                // ...or step out of it.
                if (index >= current.ScopeCount)
                    indices.Pop();
                index = indices.Pop() + 1;
                indices.Push(index);
                current = symbols.global.GetScope(index);
            }
            return result;
        }

        // public void Walk(Action<SymbolTable.Scope> action) { }
    }
}
