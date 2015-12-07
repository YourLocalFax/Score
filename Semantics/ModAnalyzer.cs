using Log;
using Symbols;
using SyntaxTree;

namespace Semantics
{
    internal sealed class ModAnalyzer : IAstVisitor
    {
        private readonly DetailLogger log;
        private readonly SymbolTable symbols;

        public ModAnalyzer(DetailLogger log, SymbolTable symbols)
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

        public void Visit(NodeLet let)
        {
            log.Error(let.Span, "A let binding is not valid in this placement. Perhaps you meant to put it in a function?");
        }

        public void Visit(NodeId id)
        {
            log.Error(id.Span, "An identifier is not valid in this placement. Perhaps you meant to put it in a function?");
        }

        public void Visit(NodeBool b)
        {
            log.Error(b.Span, "A bool literal is not valid in this placement. Perhaps you meant to put it in a function?");
        }

        public void Visit(NodeInt i)
        {
            log.Error(i.Span, "An int literal is not valid in this placement. Perhaps you meant to put it in a function?");
        }

        public void Visit(NodeStr s)
        {
            log.Error(s.Span, "A string literal is not valid in this placement. Perhaps you meant to put it in a function?");
        }

        public void Visit(NodeEnclosed enc)
        {
            log.Error(enc.Span, "A string literal is not valid in this placement. Perhaps you meant to put it in a function?");
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

        public void Visit(NodeIf @if)
        {
            log.Error(@if.Span, "Expressions are not valid in this placement. Perhaps you meant to put it in a function?");
        }
    }
}
