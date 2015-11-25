using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Lex
{
    internal sealed class TokenList
    {
        private readonly Token[] tokens;

        private int index;

        // TODO(kai): THIS IS DANGER, FIXIT
        public Token Last => tokens[index - 1];
        public Token Current => tokens[index];
        public Token Next => Peek(1);

        public bool HasLast => index > 0;
        public bool HasCurrent => index < tokens.Length;
        public bool HasNext => index + 1 < tokens.Length;

        public TokenList(List<Token> tokens)
        {
            this.tokens = tokens.ToArray();
        }

        public Token SetCurrent(Token token)
        {
            var result = Current;
            tokens[index] = token;
            return result;
        }

        public void Advance()
        {
            index++;
        }

        public Token Peek(int offset)
        {
            if (offset <= 0)
                throw new ArgumentException("offset must be positive.");
            if (!HasCurrent || index + offset >= tokens.Length)
                return null;
            return tokens[index + offset];
        }
    }
}
