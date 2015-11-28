namespace Score.Front.Parse.Ty
{
    using static System.Diagnostics.Debug;

    using Data;
    using Lex;

    internal abstract class TyVariant
    {
        public static TyVariant GetForPrimitive(TokenPrimitiveTyName token)
        {
            var span = token.span;
            var image = token.Image;
            // TODO(kai): Maybe change this up some.
            switch (image)
            {
                case "bool": return new TyBool(span);
                case "i8": return new TyInt8(span);
                case "u8": return new TyUint8(span);
                case "i16": return new TyInt16(span);
                case "u16": return new TyUint16(span);
                case "i32": return new TyInt32(span);
                case "u32": return new TyUint32(span);
                case "i64": return new TyInt64(span);
                case "u64": return new TyUint64(span);
                case "f16": return new TyFloat16(span);
                case "f32": return new TyFloat32(span);
                case "f64": return new TyFloat64(span);
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

        public abstract Span Span { get; }

        public override abstract string ToString();
    }
}
