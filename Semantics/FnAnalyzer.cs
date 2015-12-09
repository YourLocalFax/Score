using Log;
using Symbols;
using SyntaxTree;

namespace Semantics
{
    // TODO(kai): Make sure that all code paths return a value (if not void)
    // TODO(kai): The type checking portion can be done later.

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
            log.Error(fn.Span, "A function is not valid in another function.");
        }

        public void Visit(NodeTypeDef typeDef)
        {
            log.Error(typeDef.Span, "A type def is not valid in a function.");
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
            enc.expr.Accept(this);
        }

        public void Visit(NodeIndex index)
        {
            index.target.Accept(this);
            index.index.Accept(this);
        }

        public void Visit(NodeInfix infix)
        {
            infix.left.Accept(this);
            infix.right.Accept(this);
        }

        public void Visit(NodeSuffix suffix)
        {
            suffix.target.Accept(this);
        }

        public void Visit(NodeTuple tuple)
        {
            tuple.values.ForEach(value => value.Accept(this));
        }

        public void Visit(NodeStr s)
        {
        }

        public void Visit(NodeBool b)
        {
        }

        public void Visit(NodeRet ret)
        {
            ret.value.Accept(this);
        }

        public void Visit(NodeIf @if)
        {
        }
    }
}
