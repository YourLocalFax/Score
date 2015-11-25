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

    internal class ModAnalyzer : IAstVisitor
    {
        private readonly DetailLogger log;
        private readonly SymbolTable symbols;

        public ModAnalyzer(DetailLogger log, SymbolTable symbols)
        {
            this.log = log;
            this.symbols = symbols;
        }

        public void Visit(Ast node)
        {
            node.children.ForEach(child => child.Accept(this));
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
            var analyzer = new FnAnalyzer(log, symbols);
            fn.body.ForEach(node => node.Accept(analyzer));
            symbols.ExitScope();
        }

        public void Visit(NodeId id)
        {
            log.Error(id.Span, "An identifier is not valid in this placement. Perhaps you meant to put it in a function?");
        }

        public void Visit(NodeInt i)
        {
            log.Error(i.Span, "An int literal is not valid in this placement. Perhaps you meant to put it in a function?");
        }

        public void Visit(NodeStr s)
        {
            log.Error(s.Span, "A string literal is not valid in this placement. Perhaps you meant to put it in a function?");
        }

        public void Visit(NodeTuple tuple)
        {
            log.Error(tuple.Span, "A tuple is not valid in this placement. Perhaps you meant to put it in a function?");
        }

        public void Visit(NodeIndex index)
        {
            log.Error(index.Span, "Expressions are not valid in this placement. Perhaps you meant to put it in a function?");
        }

        public void Visit(NodeInvoke invoke)
        {
            log.Error(invoke.Span, "Expressions are not valid in this placement. Perhaps you meant to put it in a function?");
        }

        public void Visit(NodeInfix infix)
        {
            log.Error(infix.Span, "Expressions are not valid in this placement. Perhaps you meant to put it in a function?");
        }

        public void Visit(NodeSuffix suffix)
        {
            log.Error(suffix.Span, "Expressions are not valid in this placement. Perhaps you meant to put it in a function?");
        }
    }
}
