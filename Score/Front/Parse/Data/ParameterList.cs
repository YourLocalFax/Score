using System.Collections.Generic;

namespace Score.Front.Parse.Data
{
    using Lex;

    internal sealed class ParameterList
    {
        public TokenOp leadingPipe;
        public readonly List<Parameter> parameters = new List<Parameter>();
        public TokenOp trailingPipe;
    }
}
