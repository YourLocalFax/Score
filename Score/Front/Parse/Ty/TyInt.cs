using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score.Front.Parse.Ty
{
    internal abstract class TyInt : TyPrimitive
    {
        public TyInt(Span span) : base(span) { }

        public abstract uint BitWidth { get; }

        public override string ToString() => "i" + BitWidth;

    }

    internal sealed class TyInt8 : TyInt
    {
        public TyInt8(Span span) : base(span) { }

        public override uint BitWidth => 8;

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            Int8TypeInContext(context);
    }

    internal sealed class TyInt16 : TyInt
    {
        public TyInt16(Span span) : base(span) { }

        public override uint BitWidth => 16;

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            Int16TypeInContext(context);
    }

    internal sealed class TyInt32 : TyInt
    {
        public TyInt32(Span span) : base(span) { }

        public override uint BitWidth => 32;

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            Int32TypeInContext(context);
    }

    internal sealed class TyInt64 : TyInt
    {
        public TyInt64(Span span) : base(span) { }

        public override uint BitWidth => 64;

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            Int64TypeInContext(context);
    }
}
