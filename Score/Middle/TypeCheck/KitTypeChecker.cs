namespace Score.Middle.TypeCheck
{
    using Symbols;

    /// <summary>
    /// Compiles entire programs.
    /// </summary>
    internal sealed class KitTypeChecker : ModTypeChecker
    {
        public KitTypeChecker(DetailLogger log, SymbolTableWalker walker)
            : base(log, walker)
        {
        }
    }
}
