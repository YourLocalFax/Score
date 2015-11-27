namespace Score.Front.Parse.Data
{
    using Lex;

    internal sealed class Op
    {
        public readonly TokenOp op;

        public string Image => op.Image;

        public Op(TokenOp op)
        {
            this.op = op;
        }
    }
}
