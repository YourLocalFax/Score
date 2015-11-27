namespace Score.Front.Parse.Ty
{
    internal abstract class TyRef
    {
        public static BaseTyRef Void(Span span) => new BaseTyRef(TyOrVoid.FromVoid(span));
        public static BaseTyRef For(TyOrVoid ty) => new BaseTyRef(ty);
        public static PointerTyRef PointerTo(TyRef ty, bool isMut) => new PointerTyRef(ty, isMut);
        public static ReferenceTyRef ReferenceTo(TyRef ty, bool isMut) => new ReferenceTyRef(ty, isMut);
        public static ArrayTyRef ArrayOf(TyRef inner, uint depth) => new ArrayTyRef(inner, depth);
        public static TupleTyRef TupleOf(params TyRef[] inner) => new TupleTyRef(inner);

        public bool IsVoid => (this as BaseTyRef)?.ty.IsVoid ?? false;
    }

    internal sealed class BaseTyRef : TyRef
    {
        public readonly TyOrVoid ty;

        public BaseTyRef(TyOrVoid ty)
        {
            this.ty = ty;
        }
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
    }

    internal sealed class TupleTyRef : TyRef
    {
        public readonly TyRef[] inner;

        public TupleTyRef(params TyRef[] inner)
        {
            this.inner = inner;
        }
    }
}
