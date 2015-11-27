namespace Score.Front.Parse.Data
{
    using Lex;

    internal sealed class Op
    {
        public readonly TokenOp op;

        public Op(TokenOp op)
        {
            this.op = op;
        }
    }
}
