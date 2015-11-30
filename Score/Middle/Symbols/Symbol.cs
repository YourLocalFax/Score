using LLVMSharp;

namespace Score.Middle.Symbols
{
    using System;
    using Front.Parse.Data;
    using Front.Parse.Ty;

    internal abstract class Symbol
    {
        public enum Kind
        {
            TYPE,
            FN,
            VAR,
            // TODO(kai): other data types that need it
        }

        public readonly string name;
        public readonly Kind kind;

        public abstract TyRef Ty { get; }

        public Symbol(string name, Kind kind)
        {
            this.name = name;
            this.kind = kind;
        }

        public override string ToString() => name;
    }

    internal sealed class FnSymbol : Symbol
    {
        public readonly Modifiers mods;
        public readonly TyFn ty;
        public readonly LLVMValueRef llvmFn;

        public override TyRef Ty => ty;

        public FnSymbol(string name, Modifiers mods, TyFn ty, LLVMValueRef llvmFn)
            : base(name, Kind.FN)
        {
            this.mods = mods;
            this.ty = ty;
            this.llvmFn = llvmFn;
        }
    }

    internal sealed class TypeSymbol : Symbol
    {
        public readonly Modifiers mods;
        public readonly TyRef ty;

        public override TyRef Ty => ty;

        public TypeSymbol(string name, Modifiers mods, TyRef ty)
            : base(name, Kind.TYPE)
        {
            this.mods = mods;
            this.ty = ty;
        }
    }

    internal class VarSymbol : Symbol
    {
        public readonly TyRef ty;
        public readonly LLVMValueRef pointer;
        public readonly bool isMut;

        public override TyRef Ty => ty;

        public VarSymbol(string name, TyRef ty, LLVMValueRef pointer, bool isMut)
            : base(name, Kind.VAR)
        {
            this.ty = ty;
            this.pointer = pointer;
            this.isMut = isMut;
        }
    }
}
