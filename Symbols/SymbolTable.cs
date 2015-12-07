using System.Collections.Generic;
using System.Text;

using Ext;
using SyntaxTree.Data;
using Ty;

namespace Symbols
{
    public sealed class SymbolTable
    {
        public sealed class Scope
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

            public void InsertFn(string fnName, Modifiers mods, FnTyRef ty) =>
                symbols[fnName] = new FnSymbol(fnName, mods, ty);

            public void InsertType(string typeName, Modifiers mods, TyRef ty) =>
                symbols[typeName] = new TypeSymbol(typeName, mods, ty);

            public void InsertVar(string varName, TyRef ty, bool isMut) =>
                symbols[varName] = new VarSymbol(varName, ty, isMut);

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
                new List<string>(symbols.Keys).ForEach(symbol =>
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


        public void InsertFn(string fnName, Modifiers mods, FnTyRef ty) =>
            current.InsertFn(fnName, mods, ty);

        public void InsertType(string typeName, Modifiers mods, TyRef ty) =>
            current.InsertType(typeName, mods, ty);

        public void InsertVar(string varName, TyRef ty) =>
            current.InsertVar(varName, ty, false);

        //public Symbol Lookup(string name) => current.Lookup(name);

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
