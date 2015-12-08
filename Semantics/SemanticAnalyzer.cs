using Log;
using Symbols;
using SyntaxTree;

namespace Semantics
{
    /// <summary>
    /// Handles the semantic analysis phase, generating an AST in the process.
    /// </summary>
    public sealed class SemanticAnalyzer
    {
        private readonly DetailLogger log;

        public SemanticAnalyzer(DetailLogger log)
        {
            this.log = log;
        }

        public SymbolTable Analyze(Ast ast)
        {
            var symbols = new SymbolTable();
            ast.Accept(new ModAnalyzer(log, symbols));
            // TODO(kai): Check that variABLES ACTUALLY EXIST
            return symbols;
        }
    }
}
