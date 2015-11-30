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
        public readonly Token arrow; // TODO(kai): remove this
        // NOTE(kai): If this is null, we infer the type.
        // TODO(kai): have a specific "infer" type eventually.
        public readonly Parameter returnParameter;

        public TyFn(ParameterList parameters, Parameter returnParameter, Token arrow)
        {
            this.parameters = parameters;
            this.returnParameter = returnParameter;
            this.arrow = arrow;
        }

        public override LLVMTypeRef GetLLVMTy(LLVMContextRef context) =>
            FunctionType(returnParameter.ty.GetLLVMTy(context), parameters.Select(param => param.ty.GetLLVMTy(context)).ToArray(), false);

        public override bool SameAs(TyRef ty)
        {
            var fn = ty as TyFn;
            if (fn == null)
                return false;
            if (fn.parameters.Count != parameters.Count)
                return false;
            if (!returnParameter.ty.SameAs(fn.returnParameter.ty))
                return false;
            for (int i = 0, len = parameters.Count; i < len; i++)
                if (!fn.parameters[i].ty.SameAs(parameters[i].ty))
                    return false;
            return true;
        }
    }
}
