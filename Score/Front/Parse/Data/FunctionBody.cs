using System.Collections.Generic;

namespace Score.Front.Parse.Data
{
    using Lex;
    using SyntaxTree;

    internal sealed class FunctionBody
    {
        public Token eq;
        public Token lbrace;
        public readonly List<Node> body = new List<Node>();
        public Token rbrace;
    }
}
