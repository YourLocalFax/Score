using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LLVMSharp;

using static LLVMSharp.LLVM;

namespace Score.Back.Types
{
    using Front;
    using Front.Parse;
    using Front.Parse.Types;
    using Types;

    internal abstract class ScoreType
    {
        public static ScoreType TempGetType(DetailLogger log, TypeInfo typeInfo)
        {
            if (typeInfo == TypeInfo.VOID)
                return new ScoreTupleType();
            if (typeInfo is PointerTypeInfo)
            {
                return new ScorePointerType(TempGetType(log, (typeInfo as PointerTypeInfo).type),
                    (typeInfo as PointerTypeInfo).isMut);
            }
            else if (typeInfo is PathTypeInfo)
            {
                switch ((typeInfo as PathTypeInfo).path.path.Single())
                {
                    case "i8": return new ScoreIntType(8, false);
                    case "i32": return new ScoreIntType(32, false);
                }
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
