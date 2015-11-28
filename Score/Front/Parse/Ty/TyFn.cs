using System.Linq;

using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score.Front.Parse.Ty
{
    using Data;
    using Lex;

    internal sealed class TyFn : TyRef
    {
        public readonly ParameterList parameters;
        public readonly Token arrow;
        // NOTE(kai): If this is null, we infer the type.
        // TODO(kai): have a specific "infer" type eventually.
        public readonly TyRef returnTy;

        public TyFn(ParameterList parameters, TyRef returnTy, Token arrow)
        {
            this.parameters = parameters;
            this.returnTy = returnTy;
            this.arrow = arrow;
        }

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            FunctionType(returnTy.GetLLVMTy(context), parameters.Select(param => param.ty.GetLLVMTy(context)).ToArray(), false);
    }
}
