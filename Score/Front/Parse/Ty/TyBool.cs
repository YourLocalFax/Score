using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score.Front.Parse.Ty
{
    internal sealed class TyBool : TyPrimitive
    {
        public override string ToString() => "bool";

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            Int1TypeInContext(context);
    }
}
