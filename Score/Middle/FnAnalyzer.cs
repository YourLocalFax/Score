using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Middle
{
    using Front.Parse;
    using Front.Parse.SyntaxTree;

    using Symbols;

    internal sealed class FnAnalyzer : IAstVisitor
    {
        private readonly DetailLogger log;
        private readonly SymbolTable symbols;

        public FnAnalyzer(DetailLogger log, SymbolTable symbols)
        {
            this.log = log;
            this.symbols = symbols;
        }

        public void Visit(Ast node)
        {
            // TODO(kai): this is not valid, error on it.
        }

        public void Visit(NodeFnDecl fnDecl)
        {
            // FIXME(kai): type information, please <3
            symbols.Insert(fnDecl.name, Symbol.Kind.FN, null, fnDecl.mods);
        }

        public void Visit(NodeFn fn)
        {
            fn.decl.Accept(this);
            symbols.NewScope();
            // NOTE(kai): re-using this analyzer because why not, shouldn't hurt.
            fn.body.ForEach(node => node.Accept(this));
            symbols.ExitScope();
        }

        public void Visit(NodeId id)
        {
            // NOTE(kai): do nothing.
        }

        public void Visit(NodeInt i)
        {
            // NOTE(kai): do nothing.
        }

        public void Visit(NodeStr s)
        {
            // NOTE(kai): do nothing.
        }

        public void Visit(NodeTuple tuple)
        {
            tuple.values.ForEach(value => value.Accept(this));
        }

        public void Visit(NodeIndex index)
        {
            index.target.Accept(this);
            index.index.Accept(this);
        }

        public void Visit(NodeInvoke invoke)
        {
            invoke.target.Accept(this);
            invoke.args.ForEach(arg => arg.Accept(this));
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
    }
}
