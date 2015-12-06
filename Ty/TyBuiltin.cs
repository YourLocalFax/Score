using System.Diagnostics;

namespace Ty
{
    public abstract class BuiltinTyRef : TyRef
    {
        public static BuiltinTyRef GetForPrimitive(string name)
        {
            switch (name)
            {
                case "bool": return TyBool.BoolTy;
                case "i8":   return TyInt.Int8Ty;
                case "u8":   return TyUint.Uint8Ty;
                case "i16":  return TyInt.Int16Ty;
                case "u16":  return TyUint.Uint16Ty;
                case "i32":  return TyInt.Int32Ty;
                case "u32":  return TyUint.Uint32Ty;
                case "i64":  return TyInt.Int64Ty;
                case "u64":  return TyUint.Uint64Ty;
                case "f16":  return TyFloat.Float16Ty;
                case "f32":  return TyFloat.Float32Ty;
                case "f64":  return TyFloat.Float64Ty;
                default:
                    Debug.Assert(false, "This should never happen. If it does, you missed a case for primitive types.");
                    return null;
            }
        }
    }

    public sealed class TyVoid : BuiltinTyRef
    {
        public static readonly TyVoid VoidTy = new TyVoid();

        private TyVoid() { }

        public override string ToString() => "()";
    }

    public sealed class TyBool : BuiltinTyRef
    {
        public static readonly TyBool BoolTy = new TyBool();

        private TyBool() { }

        public override string ToString() => "bool";
    }

    public class TyInt : BuiltinTyRef
    {
        public static readonly TyInt Int8Ty = new TyInt(8);
        public static readonly TyInt Int16Ty = new TyInt(16);
        public static readonly TyInt Int32Ty = new TyInt(32);
        public static readonly TyInt Int64Ty = new TyInt(64);

        private TyInt(uint bitWidth) { BitWidth = bitWidth; }

        public uint BitWidth { get; private set; }

        public override string ToString() => "i" + BitWidth;
    }

    public class TyUint : BuiltinTyRef
    {
        public static readonly TyUint Uint8Ty = new TyUint(8);
        public static readonly TyUint Uint16Ty = new TyUint(16);
        public static readonly TyUint Uint32Ty = new TyUint(32);
        public static readonly TyUint Uint64Ty = new TyUint(64);

        private TyUint(uint bitWidth) { BitWidth = bitWidth; }

        public uint BitWidth { get; private set; }

        public override string ToString() => "i" + BitWidth;
    }

    public sealed class TyFloat : BuiltinTyRef
    {
        public static readonly TyFloat Float16Ty = new TyFloat(16);
        public static readonly TyFloat Float32Ty = new TyFloat(32);
        public static readonly TyFloat Float64Ty = new TyFloat(64);

        private TyFloat(uint bitWidth) { BitWidth = bitWidth; }

        public uint BitWidth { get; private set; }

        public override string ToString() => "f" + BitWidth;
    }
}
