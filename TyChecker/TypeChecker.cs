using Log;
using Symbols;
using SyntaxTree;

namespace TyChecker
{
    public class TypeChecker
    {
        private readonly DetailLogger log;

        public TypeChecker(DetailLogger log)
        {
            this.log = log;
        }

        public void Check(Ast ast, SymbolTable symbols)
        {
            var resolver = new TyResolver(log);
            resolver.Resolve(ast, symbols);

            if (log.HasErrors)
                return;


        }
    }
}
