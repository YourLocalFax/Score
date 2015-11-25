using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse
{
    using Types;

    // TODO(kai): Everything that we need should be here soon
    internal sealed class ParamInfo
    {
        // Params need a name, mutability, and a type.
        public string name;
        public Span nameSpan;

        public bool mut;

        public TypeInfo type;
        public Span typeSpan;
    }
}
