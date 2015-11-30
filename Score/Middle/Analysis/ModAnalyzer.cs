namespace Score.Middle.Analysis
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

        public void Visit(NodeFnDecl fn)
        {
            var mods = fn.header.modifiers;

            var dups = mods.GetDuplicates();
            if (dups.Count > 0)
            {
                dups.ForEach(dup =>
                {
                    log.Error(dup.span, "Duplicate modifier '{0}'.", dup.Image);
                });
            }

            var conflicts = mods.GetConflicting();
            if (conflicts.Count > 0)
            {
                // TODO(kai): maybe make this one more descriptive. What does it conflict with?
                dups.ForEach(dup =>
                {
                    log.Error(dup.span, "Conflicting modifier '{0}'.", dup.Image);
                });
            }

            if (mods.Has(Front.Lex.Token.Type.EXTERN))
            {
                // TODO(kai): 'extern' functions CAN have a body, but not now...
                if (fn.body != null)
                    log.Error(mods.GetSpan(Front.Lex.Token.Type.EXTERN),
                        "Currently, extern functions cannot have a body.");
            }

            symbols.Insert(fn.Name, Symbol.Kind.FN, fn.ty, fn.header.modifiers);
            if (fn.body != null)
            {
                symbols.NewScope(fn.Name);
                var analyzer = new FnAnalyzer(log, symbols);
                fn.body.ForEach(node => node.Accept(analyzer));
                symbols.ExitScope();
            }
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
