using System;
using System.Linq;

using Ty;

using LLVMSharp;
using static LLVMSharp.LLVM;

namespace CodeGen
{
    internal static class TypeConverter
    {
        public static LLVMTypeRef ToLLVMTy(TyRef ty, LLVMContextRef context)
        {
            if (ty is TyInt)
                return IntTypeInContext(context, (ty as TyInt).BitWidth);
            else if (ty is TyUint)
                return IntTypeInContext(context, (ty as TyUint).BitWidth);
            else if (ty is TyFloat)
            {
                switch ((ty as TyFloat).BitWidth)
                {
                    case 16: return HalfTypeInContext(context);
                    case 32: return FloatTypeInContext(context);
                    case 64: return DoubleTypeInContext(context);
                    // EW
                    default: throw new ArgumentException("ty");
                }
            }
            else if (ty is TyBool)
                return Int1TypeInContext(context);
            else if (ty is TyVoid)
                return VoidTypeInContext(context);
            else if (ty is PointerTyRef)
                return PointerType(ToLLVMTy((ty as PointerTyRef).ty.Raw, context), 0);
            else if (ty is FnTyRef)
            {
                var fnTy = ty as FnTyRef;
                var llvmTyParams = fnTy.parameterTys.Select(param => ToLLVMTy(param.Raw, context)).ToArray();
                var llvmTyReturn = ToLLVMTy(fnTy.returnTy.Raw, context);
                return FunctionType(llvmTyReturn, llvmTyParams, false);
            }
            // TODO(kai): structs and things go here pls
            else throw new ArgumentException("ty");
        }
    }
}
