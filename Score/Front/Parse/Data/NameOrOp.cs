namespace Score.Front.Parse.Data
{
    /// <summary>
    /// Represents an identifier or an operator used as a name.
    /// </summary>
    internal sealed class NameOrOp
    {
        public static NameOrOp FromId(Name id) => new NameOrOp(id);
        public static NameOrOp FromOp(Op op) => new NameOrOp(op);

        public readonly Name id;
        public readonly Op op;

        public bool IsId => id != null;
        public bool IsOp => op != null;

        private NameOrOp(Name id)
        {
            this.id = id;
        }

        private NameOrOp(Op op)
        {
            this.op = op;
        }
    }
}
