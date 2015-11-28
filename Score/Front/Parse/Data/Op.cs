namespace Score.Front.Parse.Data
{
    using Lex;

    internal sealed class Op
    {
        private readonly TokenOp op;

        public Span Span => op.span;
        public string Image => op.Image;

        public Op(TokenOp op)
        {
            this.op = op;
        }
    }
}
