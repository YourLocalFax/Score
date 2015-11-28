using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LLVMSharp;

using static LLVMSharp.LLVM;

namespace Score.Back.Types
{
    using Front.Lex;
    using Front.Parse.Ty;

    internal abstract class ScoreType
    {
        public static ScoreType TempGetType(DetailLogger log, TyRef type)
        {
            if (type.IsVoid)
                return new ScoreTupleType();
            if (type is PointerTyRef)
            {
                return new ScorePointerType(TempGetType(log, (type as PointerTyRef).ty),
                    (type as PointerTyRef).isMut);
            }
            else if (type is BaseTyRef)
            {
                var baseTy = type as BaseTyRef;
                if (baseTy.ty is TyInt)
                    return new ScoreIntType((baseTy.ty as TyInt).BitWidth, false);
            }
            return null;
        }

        public abstract LLVMTypeRef TempGetLLVMType(LLVMContextRef c);
    }

    internal sealed class ScoreIntType : ScoreType
    {
        public readonly uint bits;
        public readonly bool unsigned;

        public ScoreIntType(uint bits, bool unsigned)
        {
            this.bits = bits;
            this.unsigned = unsigned;
        }

        public override LLVMTypeRef TempGetLLVMType(LLVMContextRef c)
        {
            switch (bits)
            {
                case 1: return Int1TypeInContext(c);
                case 8: return Int8TypeInContext(c);
                case 16: return Int16TypeInContext(c);
                case 32: return Int32TypeInContext(c);
                case 64: return Int64TypeInContext(c);
                default: return IntTypeInContext(c, bits);
            }
        }
    }

    internal sealed class ScorePointerType : ScoreType
    {
        public readonly ScoreType type;
        public readonly bool isMut;

        public ScorePointerType(ScoreType type, bool isMut)
        {
            Console.WriteLine(type);
            this.type = type;
            this.isMut = isMut;
        }

        public override LLVMTypeRef TempGetLLVMType(LLVMContextRef c)
        {
            return PointerType(type.TempGetLLVMType(c), 0);
        }
    }

    internal sealed class ScoreTupleType : ScoreType
    {
        public readonly ScoreType[] types;

        public ScoreTupleType(params ScoreType[] types)
        {
            this.types = types;
        }

        public override LLVMTypeRef TempGetLLVMType(LLVMContextRef c)
        {
            if (types.Length == 0)
                return VoidTypeInContext(c);
            return default(LLVMTypeRef);
        }
    }
}
