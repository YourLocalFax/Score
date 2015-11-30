using System.Collections.Generic;

using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score.Back
{
    internal sealed class GlobalStateManager
    {
        public readonly LLVMContextRef context;

        private readonly Dictionary<string, LLVMValueRef> cStrConsts = new Dictionary<string, LLVMValueRef>();

        public GlobalStateManager(LLVMContextRef context)
        {
            this.context = context;
        }

        public LLVMValueRef CStrConst(LLVMBuilderRef builder, string value, string name = "")
        {
            // FIXME(kai): things!
            return BuildGlobalStringPtr(builder, value, name);
        }
    }
}
