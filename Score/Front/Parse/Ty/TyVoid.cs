using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score.Front.Parse.Ty
{
    internal sealed class TyVoid : TyPrimitive
    {
        public override string ToString() => "()";

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            VoidTypeInContext(context);
    }
}
