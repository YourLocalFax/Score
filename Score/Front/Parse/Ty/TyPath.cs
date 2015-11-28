using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse.Ty
{
    using Data;

    internal sealed class TyPath : TyVariant
    {
        private QualifiedNameWithTyArgs nameWithTyArgs;

        public QualifiedName Name => nameWithTyArgs.name;

        public override Span Span => nameWithTyArgs.Span;

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
