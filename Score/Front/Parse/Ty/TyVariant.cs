using static System.Diagnostics.Debug;

using LLVMSharp;

namespace Score.Front.Parse.Ty
{
    using Data;
    using Lex;

    internal abstract class TyVariant
    {
        public static TyVariant GetForPrimitive(TokenPrimitiveTyName token)
        {
            var image = token.Image;
            // TODO(kai): Maybe change this up some.
            switch (image)
            {
                case "bool": return new TyBool();
                case "i8": return new TyInt8();
                case "u8": return new TyUint8();
                case "i16": return new TyInt16();
                case "u16": return new TyUint16();
                case "i32": return new TyInt32();
                case "u32": return new TyUint32();
                case "i64": return new TyInt64();
                case "u64": return new TyUint64();
                case "f16": return new TyFloat16();
                case "f32": return new TyFloat32();
                case "f64": return new TyFloat64();
                // TODO(kai): This looks kinda bleh, maybe not do this.
                default:
                    Assert(false, "This should never happen. If it does, you missed a case for primitive types.");
                    return null;
            }
        }

        public static TyVariant GetFor(QualifiedNameWithTyArgs nameWithTyArgs)
        {
            // TODO(kai): Implement this
            return null;
        }

        // public abstract Span Span { get; }

        public override abstract string ToString();

        public abstract LLVMTypeRef GetLLVMTy(LLVMContextRef context);
    }
}
