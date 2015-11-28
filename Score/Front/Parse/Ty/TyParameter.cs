using System;

namespace Score.Front.Parse.Ty
{
    using Data;

    internal sealed class TyParameter : TyVariant
    {
        public override Span Span
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TyParameter(Span span, Name name) { }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
