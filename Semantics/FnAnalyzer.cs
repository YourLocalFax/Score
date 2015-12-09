using Log;
using Symbols;
using SyntaxTree;

namespace Semantics
{
    internal sealed class FnAnalyzer : IAstVisitor
    {
        private readonly DetailLogger log;
        private readonly SymbolTable symbols;

        public FnAnalyzer(DetailLogger log, SymbolTable symbols)
        {
            this.log = log;
            this.symbols = symbols;
        }

        public void Visit(Ast node) =>
            node.children.ForEach(child => child.Accept(this));

        public void Visit(NodeFnDecl fn)
        {
            symbols.InsertFn(fn.Name, fn.header.modifiers, fn.ty);
            if (fn.body != null)
            {
                symbols.NewScope(fn.Name);
                var fnAnalyzer = new FnAnalyzer(log, symbols);
                fn.body.ForEach(node => node.Accept(fnAnalyzer));
                symbols.ExitScope();
            }
        }

        public void Visit(NodeTypeDef typeDef)
        {
            symbols.InsertType(typeDef.Name, typeDef.modifiers, typeDef.Ty);
        }

        public void Visit(NodeInvoke invoke)
        {
            invoke.args.ForEach(arg => arg.Accept(this));
        }

        public void Visit(NodeLet let)
        {
            symbols.InsertVar(let.Name, let.Ty);
            let.value.Accept(this);
        }

        public void Visit(NodeId id)
        {
        }

        public void Visit(NodeInt i)
        {
        }

        public void Visit(NodeEnclosed enc)
        {
        }

        public void Visit(NodeIndex index)
        {
        }

        public void Visit(NodeInfix infix)
        {
        }

        public void Visit(NodeIf @if)
        {
        }

        public void Visit(NodeSuffix suffix)
        {
        }

        public void Visit(NodeTuple tuple)
        {
        }

        public void Visit(NodeStr s)
        {
        }

        public void Visit(NodeBool b)
        {
        }
    }
}
