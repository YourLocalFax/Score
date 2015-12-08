using LLVMSharp;

using Source;
using Ty;

namespace CodeGen
{
    internal sealed class ScoreVal
    {
        public readonly Span span;
        public readonly TyRef ty;
        public readonly LLVMValueRef value;

        public ScoreVal(Span span, TyRef ty, LLVMValueRef value)
        {
            this.ty = ty;
            this.value = value;
        }
    }
}
