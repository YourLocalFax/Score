using System;

namespace Score.Front.Parse.Ty
{
    internal abstract class TyInt : TyPrimitive
    {
        public TyInt(Span span) : base(span) { }

        public abstract uint BitWidth { get; }

        public override string ToString() => "i" + BitWidth;
    }

    internal sealed class TyInt8 : TyInt
    {
        public TyInt8(Span span) : base(span) { }

        public override uint BitWidth => 8;
    }

    internal sealed class TyInt16 : TyInt
    {
        public TyInt16(Span span) : base(span) { }

        public override uint BitWidth => 16;
    }

    internal sealed class TyInt32 : TyInt
    {
        public TyInt32(Span span) : base(span) { }

        public override uint BitWidth => 32;
    }

    internal sealed class TyInt64 : TyInt
    {
        public TyInt64(Span span) : base(span) { }

        public override uint BitWidth => 64;
    }
}
