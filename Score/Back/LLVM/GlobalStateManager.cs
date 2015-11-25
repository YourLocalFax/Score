using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LLVMSharp;

using static LLVMSharp.LLVM;

namespace Score.Back.LLVM
{
    internal sealed class GlobalStateManager
    {
        public readonly LLVMContextRef context;

        private readonly Dictionary<string, LLVMValueRef> cStrConsts = new Dictionary<string, LLVMValueRef>();

        public GlobalStateManager(LLVMContextRef context)
        {
            this.context = context;
        }

        public LLVMValueRef CStrConst(LLVMBuilderRef builder, string value)
        {
            // FIXME(kai): things!
            return BuildGlobalStringPtr(builder, value, "");
        }
    }
}
