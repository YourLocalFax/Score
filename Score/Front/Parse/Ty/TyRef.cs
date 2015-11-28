using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score.Front.Parse.Ty
{
    internal abstract class TyRef
    {
        public static BaseTyRef Void(Span span) => new BaseTyRef(new TyVoid(span));
        public static BaseTyRef For(TyVariant ty) => new BaseTyRef(ty);
        public static PointerTyRef PointerTo(TyRef ty, bool isMut) => new PointerTyRef(ty, isMut);
        // TODO(kai): Bring these back when we decide how to use them right.
        //public static ReferenceTyRef ReferenceTo(TyRef ty, bool isMut) => new ReferenceTyRef(ty, isMut);
        //public static ArrayTyRef ArrayOf(TyRef inner, uint depth) => new ArrayTyRef(inner, depth);
        //public static TupleTyRef TupleOf(params TyRef[] inner) => new TupleTyRef(inner);

        public bool IsVoid => (this as BaseTyRef)?.ty is TyVoid;

        public abstract LLVMTypeRef GetLLVMTy(LLVMContextRef context);
    }

    internal sealed class BaseTyRef : TyRef
    {
        public readonly TyVariant ty;

        public BaseTyRef(TyVariant ty)
        {
            this.ty = ty;
        }

        public override string ToString() => ty.ToString();

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

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            PointerType(ty.GetLLVMTy(context), 0);
    }

    /*
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
