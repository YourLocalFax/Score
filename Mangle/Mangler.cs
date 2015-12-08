using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ty;

namespace Mangle
{
    public static class Mangler
    {
        private static string GetTyName(TyRef ty)
        {
            if (ty is TyInt)
            {
                switch ((ty as TyInt).BitWidth)
                {
                    case 8: return "x";
                    case 16: return "s";
                    case 32: return "i";
                    case 64: return "l";
                    default: return "NO PLS";
                }
            }
            else if (ty is TyUint)
            {
                switch ((ty as TyUint).BitWidth)
                {
                    case 8: return "c";
                    case 16: return "j";
                    case 32: return "u";
                    case 64: return "w";
                    default: return "NO PLS";
                }
            }
            else if (ty is TyFloat)
            {
                switch ((ty as TyFloat).BitWidth)
                {
                    case 16: return "h";
                    case 32: return "f";
                    case 64: return "d";
                    default: return "NO PLS";
                }
            }
            else if (ty is TyBool)
                return "b";
            else if (ty is TyVoid)
                return "v";
            else if (ty is PointerTyRef)
                return "P" + GetTyName((ty as PointerTyRef).ty.Raw);
            // TODO(kai): structs and things go here pls
            else return "FAK";
        }

        public static string GetMangledName(string fnName, FnTyRef fnTy)
        {
            var builder = new StringBuilder().Append("_S");

            builder.Append(fnName.Length);
            builder.Append(fnName);

            builder.Append(fnTy.parameterTys.Count);
            for (int i = 0, len = fnTy.parameterTys.Count; i < len; i++)
                builder.Append(GetTyName(fnTy.parameterTys[i].Raw));

            builder.Append("_R");
            builder.Append(GetTyName(fnTy.returnTy.Raw));

            return builder.ToString();
        }
    }
}
