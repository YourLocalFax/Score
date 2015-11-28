namespace Score.Front.Parse.Data
{
    using Lex;

    internal sealed class Name
    {
        private readonly Span span;
        private readonly string image;

        public Span Span => span;
        public string Image => image;

        private Name(Token token)
        {
            span = token.span;
            image = token.Image;
        }

        public Name(TokenId token) : this(token as Token) { }
        public Name(TokenSym token) : this(token as Token) { }
        public Name(TokenPrimitiveTyName token) : this(token as Token) { }
    }
}
