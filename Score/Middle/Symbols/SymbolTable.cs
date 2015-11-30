using System.Collections.Generic;
using System.Text;

namespace Score.Middle.Symbols
{
    using Front.Parse.Data;
    using Front.Parse.Ty;

    internal sealed class SymbolTable
    {
        internal sealed class Scope
        {
            public readonly string name;
            public readonly Scope parent;

            private readonly List<Scope> children = new List<Scope>();
            internal readonly Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

            public int ScopeCount => children.Count;
            public Scope GetScope(int index) => index < children.Count ? children[index] : null;

            public Scope(string name, Scope parent = null)
            {
                this.name = name;
                this.parent = parent;
            }

            public void AddChild(Scope scope) => children.Add(scope);

            /// <summary>
            /// Returns true if the symbol has already been defined, false otherwise.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="kind"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public bool Insert(string name, Symbol.Kind kind, TyRef type, Modifiers mods)
            {
                // TODO(kai): has it been defined?
                symbols[name] = new Symbol(name, kind, type, mods);
                return false;
            }

            private bool GetSymbol(string name, out Symbol symbol) =>
                symbols.TryGetValue(name, out symbol);

            /// <summary>
            /// Attempts to find a symbol with the given name.
            /// Returns the symbol if it was found, null otherwise.
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public Symbol Lookup(string name)
            {
                var scope = this;
                while (scope != null)
                {
                    Symbol result;
                    if (scope.GetSymbol(name, out result))
                        return result;
                    scope = scope.parent;
                }
                return null; // no symbol defined.
            }

            public void WriteTo(StringBuilder builder, int tabs)
            {
                builder.Append("     ".Repeat(tabs - 1)).Append(name).AppendLine(": {");
                new List<Symbol>(symbols.Values).ForEach(symbol =>
                    builder.Append("     ".Repeat(tabs)).Append(symbol).AppendLine());
                children.ForEach(child => child.WriteTo(builder, tabs + 1));
                builder.Append("     ".Repeat(tabs - 1)).AppendLine("}");
            }

            public override string ToString()
            {
                var builder = new StringBuilder();
                WriteTo(builder, 1);
                return builder.ToString();
            }
        }

        public readonly Scope global = new Scope("global scope");
        private Scope current;

        public SymbolTable()
        {
            current = global;
        }

        public void Insert(string name, Symbol.Kind kind, TyRef type, Modifiers mods) => current.Insert(name, kind, type, mods);
        public Symbol Lookup(string name) => current.Lookup(name);

        public void NewScope(string name) => current.AddChild(current = new Scope(name, current));
        public void ExitScope() => current = current.parent;

        public override string ToString()
        {
            var builder = new StringBuilder();
            global.WriteTo(builder, 1);
            return builder.ToString();
        }
    }
}
