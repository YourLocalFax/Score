using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Middle.TypeCheck
{
    using Symbols;

    using Front.Parse.SyntaxTree;

    internal sealed class TypeChecker
    {
        public readonly DetailLogger log;
        public readonly SymbolTable symbols;

        public TypeChecker(DetailLogger log, SymbolTable symbols)
        {
            this.log = log;
            this.symbols = symbols;
        }

        public void Check(Ast ast)
        {
            var walker = new SymbolTableWalker(symbols);
            //walker.Step();

            var typeChecker = new KitTypeChecker(log, walker);
            ast.Accept(typeChecker);
        }
    }
}
