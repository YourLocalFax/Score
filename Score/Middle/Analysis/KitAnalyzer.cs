namespace Score.Middle.Analysis
{
    using Front.Parse;
    using Front.Parse.SyntaxTree;

    using Symbols;

    internal sealed class KitAnalyzer : ModAnalyzer
    {
        public KitAnalyzer(DetailLogger log, SymbolTable symbols)
            : base(log, symbols)
        {
        }
    }
}
