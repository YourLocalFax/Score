namespace Score.Front.Parse.Data
{
    using Lex;

    internal sealed class Name
    {
        private readonly Token token;

        public bool IsId => token is TokenId;
        public bool IsSym => token is TokenSym;
        public bool IsBuiltin => token is TokenBuiltin;

        public TokenId Id => token as TokenId;
        public TokenSym Sym => token as TokenSym;
        public TokenBuiltin Builtin => token as TokenBuiltin;

        public string Image => token.Image;

        public Name(TokenId token)
        {
            this.token = token;
        }

        public Name(TokenSym token)
        {
            this.token = token;
        }

        public Name(TokenBuiltin token)
        {
            this.token = token;
        }
    }
}
