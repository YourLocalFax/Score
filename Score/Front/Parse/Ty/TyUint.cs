using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score.Front.Parse.Ty
{
    internal abstract class TyUint : TyPrimitive
    {
        public abstract uint BitWidth { get; }

        public override string ToString() => "u" + BitWidth;
    }

    internal sealed class TyUint8 : TyUint
    {
        public override uint BitWidth => 8;

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            Int8TypeInContext(context);
    }

    internal sealed class TyUint16 : TyUint
    {
        public override uint BitWidth => 16;

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            Int16TypeInContext(context);
    }

    internal sealed class TyUint32 : TyUint
    {
        public override uint BitWidth => 32;

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            Int32TypeInContext(context);
    }

    internal sealed class TyUint64 : TyUint
    {
        public override uint BitWidth => 64;

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            Int8TypeInContext(context);
    }
}
