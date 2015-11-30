namespace Score.Middle.Analysis
{
    using Front.Parse.SyntaxTree;

    using Symbols;

    class SemanticAnalyzer
    {
        private readonly DetailLogger log;
        private readonly SymbolTable symbols;

        public SemanticAnalyzer(DetailLogger log, SymbolTable symbols)
        {
            this.log = log;
            this.symbols = symbols;
        }

        // This is per-file (i.e. per-kit).
        public void Analyze(string name, Ast ast)
        {
            symbols.Insert(name, Symbol.Kind.KIT, null, null);
            symbols.NewScope(name);
            var analyzer = new KitAnalyzer(log, symbols);
            ast.children.ForEach(child => child.Accept(analyzer));
            symbols.ExitScope();
        }
    }
}
