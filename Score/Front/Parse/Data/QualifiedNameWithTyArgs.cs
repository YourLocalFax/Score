using System.Diagnostics;

namespace Score.Front.Parse.Data
{
    using static System.Diagnostics.Debug;

    internal sealed class QualifiedNameWithTyArgs
    {
        public QualifiedName name;
        public TyArgList tyArgs;

        public Span Span
        {
            get
            {
                Assert(name.Count > 0, "This shouldn't ever happen, but just in case now you know.");
                return name[0].Span.Start + name[name.Count - 1].Span.End;
            }
        }

        public override string ToString()
        {
            // TODO(kai): add tyArgs to ToString
            return name.ToString();
        }
    }
}
