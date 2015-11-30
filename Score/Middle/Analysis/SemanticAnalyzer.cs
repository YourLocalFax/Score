using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score.Middle.Analysis
{
    using Back;

    using Front.Parse.SyntaxTree;

    using Symbols;

    internal sealed class SemanticAnalyzer
    {
        private readonly DetailLogger log;
        private readonly SymbolTable symbols;
        private readonly GlobalStateManager manager;

        public SemanticAnalyzer(DetailLogger log, SymbolTable symbols, GlobalStateManager manager)
        {
            this.log = log;
            this.symbols = symbols;
            this.manager = manager;
        }

        // This is per-file (i.e. per-kit).
        public LLVMModuleRef Analyze(string name, Ast ast)
        {
            //symbols.Insert(name, Symbol.Kind.KIT, null, null);
            //symbols.NewScope(name);

            var module = ModuleCreateWithNameInContext("score_test_module", manager.context);

            var analyzer = new KitAnalyzer(log, symbols, manager, module);
            ast.children.ForEach(child => child.Accept(analyzer));

            return module;

            //symbols.ExitScope();
        }
    }
}
