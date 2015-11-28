using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score.Front.Parse.Ty
{
    internal abstract class TyFloat : TyPrimitive
    {
        public TyFloat(Span span) : base(span) { }

        public abstract uint BitWidth { get; }

        public override string ToString() => "f" + BitWidth;
    }

    internal sealed class TyFloat16 : TyFloat
    {
        public TyFloat16(Span span) : base(span) { }

        public override uint BitWidth => 16;

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            HalfTypeInContext(context);
    }

    internal sealed class TyFloat32 : TyFloat
    {
        public TyFloat32(Span span) : base(span) { }

        public override uint BitWidth => 32;

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            FloatTypeInContext(context);
    }

    internal sealed class TyFloat64 : TyFloat
    {
        public TyFloat64(Span span) : base(span) { }

        public override uint BitWidth => 64;

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            DoubleTypeInContext(context);
    }
}
