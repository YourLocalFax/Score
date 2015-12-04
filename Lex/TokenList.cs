using System;
using System.Collections.Generic;

using Source;

namespace Lex
{
    public sealed class TokenList
    {
        private readonly Spanned<Token>[] tokens;

        private int index;

        // TODO(kai): THIS IS DANGER, FIXIT
        public Spanned<Token> Last => tokens[index - 1];
        public Spanned<Token> Current => tokens[index];
        public Spanned<Token> Next => Peek(1);

        public bool HasLast => index > 0;
        public bool HasCurrent => index < tokens.Length;
        public bool HasNext => index + 1 < tokens.Length;

        public TokenList(List<Spanned<Token>> tokens)
        {
            this.tokens = tokens.ToArray();
        }

        public Spanned<Token> SetCurrent(Spanned<Token> token)
        {
            var result = Current;
            tokens[index] = token;
            return result;
        }

        public void Advance() => index++;
        // TODO(kai): maybe add error checking to this?
        public void Backup() => index--;

        public Spanned<Token> Peek(int offset)
        {
            if (offset <= 0)
                throw new ArgumentException("offset must be positive.");
            if (!HasCurrent || index + offset >= tokens.Length)
                return null;
            return tokens[index + offset];
        }
    }
}
