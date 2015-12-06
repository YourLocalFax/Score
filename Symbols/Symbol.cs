using SyntaxTree.Data;
using Ty;

namespace Symbols
{
    public enum SymbolKind
    {
        TYPE,
        FN,
        VAR,
        // TODO(kai): other data types that need it
    }

    public abstract class Symbol
    {
        public readonly string name;
        public readonly SymbolKind kind;

        public object userdata;

        public abstract TyRef Ty { get; }

        public Symbol(string name, SymbolKind kind)
        {
            this.name = name;
            this.kind = kind;
        }

        public override string ToString() => name;
    }

    internal sealed class FnSymbol : Symbol
    {
        public readonly Modifiers mods;
        public readonly FnTyRef ty;

        public override TyRef Ty => ty;

        public FnSymbol(string name, Modifiers mods, FnTyRef ty)
            : base(name, SymbolKind.FN)
        {
            this.mods = mods;
            this.ty = ty;
        }
    }

    internal sealed class TypeSymbol : Symbol
    {
        public readonly Modifiers mods;
        public readonly TyRef ty;

        public override TyRef Ty => ty;

        public TypeSymbol(string name, Modifiers mods, TyRef ty)
            : base(name, SymbolKind.TYPE)
        {
            this.mods = mods;
            this.ty = ty;
        }
    }

    internal class VarSymbol : Symbol
    {
        public readonly TyRef ty;
        public readonly bool isMut;

        public override TyRef Ty => ty;

        public VarSymbol(string name, TyRef ty, bool isMut)
            : base(name, SymbolKind.VAR)
        {
            this.ty = ty;
            this.isMut = isMut;
        }
    }
}
