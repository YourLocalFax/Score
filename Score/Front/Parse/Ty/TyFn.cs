using System.Linq;

using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score.Front.Parse.Ty
{
    using Data;
    using Lex;

    // TODO(kai): move to TyRef file
    internal sealed class TyFn : TyRef
    {
        public readonly ParameterList parameters;
        // NOTE(kai): If this is null, we infer the type.
        // TODO(kai): have a specific "infer" type eventually.
        public readonly Parameter returnParameter;

        public TyFn(ParameterList parameters, Parameter returnParameter)
        {
            this.parameters = parameters;
            this.returnParameter = returnParameter;
        }

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            FunctionType(returnParameter.Ty.GetLLVMTy(context), parameters.Select(param => param.Ty.GetLLVMTy(context)).ToArray(), false);

        public override bool SameAs(TyRef ty)
        {
            var fn = ty as TyFn;
            if (fn == null)
                return false;
            if (fn.parameters.Count != parameters.Count)
                return false;
            if (!returnParameter.Ty.SameAs(fn.returnParameter.Ty))
                return false;
            for (int i = 0, len = parameters.Count; i < len; i++)
                if (!fn.parameters[i].Ty.SameAs(parameters[i].Ty))
                    return false;
            return true;
        }
    }
}
