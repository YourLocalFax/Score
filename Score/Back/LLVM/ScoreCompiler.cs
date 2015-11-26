using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LLVMSharp;

using static LLVMSharp.LLVM;

namespace Score.Back.LLVM
{
    using Front.Parse;
    using Front.Parse.SyntaxTree;

    using Middle.Symbols;

    /// <summary>
    /// Compiles entire programs.
    /// </summary>
    internal sealed class ScoreCompiler
    {
        public readonly DetailLogger log;
        public readonly SymbolTable symbols;

        public readonly GlobalStateManager manager;
        public readonly LLVMModuleRef module;

        public ScoreCompiler(DetailLogger log, SymbolTable symbols)
        {
            this.log = log;
            this.symbols = symbols;

            manager = new GlobalStateManager(ContextCreate());
            module = ModuleCreateWithNameInContext("score_test", manager.context);
        }

        public void Compile(Ast ast)
        {
            var compiler = new KitCompiler(log, manager, module, new SymbolTableWalker(symbols));
            ast.Accept(compiler);

            DumpModule(module);
            WriteBitcodeToFile(module, "../../../TEST_FILES/test.bc");
        }
    }
}
