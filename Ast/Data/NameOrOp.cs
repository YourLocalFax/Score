namespace Ast.Data
{
    /// <summary>
    /// Represents an identifier or an operator used as a name.
    /// </summary>
    public sealed class NameOrOp
    {
        public static NameOrOp FromName(string image) => new NameOrOp(image, true);
        public static NameOrOp FromOp(string image) => new NameOrOp(image, false);

        private readonly bool isName;

        public bool IsId => isName;
        public bool IsOp => !isName;

        public string Image { get; private set; }

        private NameOrOp(string image, bool isName)
        {
            Image = image;
        }

        public override string ToString() => Image;
    }
}
