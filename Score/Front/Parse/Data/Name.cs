namespace Score.Front.Parse.Data
{
    using Lex;

    internal sealed class Name
    {
        public readonly Token token;

        public bool IsId => token is TokenId;
        public bool IsSym => token is TokenSym;
        public bool IsBuiltin => token is TokenBuiltin;

        public string Image => token.ToString();

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
