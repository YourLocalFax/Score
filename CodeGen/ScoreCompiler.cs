using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Log;
using Symbols;
using SyntaxTree;

using LLVMSharp;
using static LLVMSharp.LLVM;

namespace CodeGen
{
    public sealed class ScoreCompiler
    {
        private readonly DetailLogger log;

        public ScoreCompiler(DetailLogger log)
        {
            this.log = log;
        }

        public LLVMModuleRef Compile(string name, Ast ast, SymbolTable symbols)
        {
            var context = ContextCreate();
            var module = ModuleCreateWithNameInContext(name, context);
            var builder = CreateBuilderInContext(context);

            var compiler = new ModCompiler(log, new SymbolTableWalker(symbols), context, module, builder);
            ast.Accept(compiler);

            return module;
        }
    }
}
