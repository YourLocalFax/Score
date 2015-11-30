using LLVMSharp;
using Score.Front.Parse.Data;
using System;
using static LLVMSharp.LLVM;

namespace Score.Front.Parse.Ty
{
    using Middle.Symbols;

    internal abstract class TyRef
    {
        public static readonly TyRef VoidTy = For(new TyVoid());
        public static readonly TyRef BoolTy = For(new TyBool());

        public static readonly TyRef Int8Ty = For(new TyInt8());
        public static readonly TyRef Int16Ty = For(new TyInt16());
        public static readonly TyRef Int32Ty = For(new TyInt32());
        public static readonly TyRef Int64Ty = For(new TyInt64());

        public static readonly TyRef Uint8Ty = For(new TyUint8());
        public static readonly TyRef Uint16Ty = For(new TyUint16());
        public static readonly TyRef Uint32Ty = For(new TyUint32());
        public static readonly TyRef Uint64Ty = For(new TyUint64());

        public static readonly TyRef Float16Ty = For(new TyFloat16());
        public static readonly TyRef Float32Ty = For(new TyFloat32());
        public static readonly TyRef Float64Ty = For(new TyFloat64());

        public static BaseTyRef For(TyVariant ty) => new BaseTyRef(ty);
        public static PointerTyRef PointerTo(TyRef ty, bool isMut) => new PointerTyRef(ty, isMut);
        // TODO(kai): Bring these back when we decide how to use them right.
        //public static ReferenceTyRef ReferenceTo(TyRef ty, bool isMut) => new ReferenceTyRef(ty, isMut);
        //public static ArrayTyRef ArrayOf(TyRef inner, uint depth) => new ArrayTyRef(inner, depth);
        //public static TupleTyRef TupleOf(params TyRef[] inner) => new TupleTyRef(inner);

        public bool IsVoid => (this as BaseTyRef)?.ty is TyVoid;

        // TODO(kai): These things will fail when trying to infer the type.
        // Make sure that this is only called when types are updated after inference.
        public abstract LLVMTypeRef GetLLVMTy(LLVMContextRef context);

        // FIXME(kai): This is probably really stupid
        public abstract bool SameAs(TyRef ty);
    }

    internal sealed class BaseTyRef : TyRef
    {
        public readonly TyVariant ty;

        public BaseTyRef(TyVariant ty)
        {
            this.ty = ty;
        }

        public override string ToString() => ty.ToString();

        public override bool SameAs(TyRef ty) => (ty as BaseTyRef)?.ty.GetType() == this.ty.GetType();

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            ty.GetLLVMTy(context);
    }

    internal sealed class PointerTyRef : TyRef
    {
        public readonly TyRef ty;
        public readonly bool isMut;

        public PointerTyRef(TyRef ty, bool isMut)
        {
            this.ty = ty;
            this.isMut = isMut;
        }

        public override string ToString() =>
            string.Format("^{0}{1}", isMut ? "mut " : "", ty.ToString());

        public override bool SameAs(TyRef ty)
        {
            var pty = ty as PointerTyRef;
            if (pty == null)
                return false;
            return (pty.isMut == isMut) && pty.ty.SameAs(this.ty);
        }

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            PointerType(ty.GetLLVMTy(context), 0);
    }

    // TODO(kai): this is a temporary path type, so shh
    internal sealed class PathTyRef : TyRef
    {
        public readonly Span span;
        public readonly string name;

        public PathTyRef(Span span, string name)
        {
            this.span = span;
            this.name = name;
        }

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context)
        {
            throw new NotImplementedException();
        }

        public override bool SameAs(TyRef ty)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => name;
    }

    /*
    internal sealed class PathTyRef : TyRef
    {
        public readonly QualifiedName name;

        public PathTyRef(QualifiedName name)
        {
            this.name = name;
        }

        public override string ToString() => name.ToString();

        // TODO(kai): I don't like this, maybe change things.

        public override bool SameAs(TyRef ty) { throw new NotImplementedException(); }

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) { throw new NotImplementedException(); }
    }

    internal sealed class ReferenceTyRef : TyRef
    {
        public readonly TyRef ty;
        public readonly bool isMut;

        public ReferenceTyRef(TyRef ty, bool isMut)
        {
            this.ty = ty;
            this.isMut = isMut;
        }

        public override string ToString() =>
            string.Format("&{0}{1}", isMut ? "mut " : "", ty.ToString());
    }

    internal sealed class ArrayTyRef : TyRef
    {
        public readonly TyRef inner;
        public readonly uint depth;

        public ArrayTyRef(TyRef inner, uint depth)
        {
            this.inner = inner;
            this.depth = depth;
        }

        public override string ToString() =>
            string.Format("[{0}{1}]", inner.ToString(), ",".Repeat((int)depth - 1));
    }

    internal sealed class TupleTyRef : TyRef
    {
        public readonly TyRef[] inner;

        public TupleTyRef(params TyRef[] inner)
        {
            this.inner = inner;
        }

        public override string ToString()
        {
            var builder = new StringBuilder().Append('(');

            // TODO(kai): probably a better way to do this, never thought about it.
            for (int i = 0; i < inner.Length; i++)
            {
                if (i > 0)
                    builder.Append(", ");
                builder.Append(inner[i]);
            }

            return builder.Append(')').ToString();
        }
    }
    */
}
