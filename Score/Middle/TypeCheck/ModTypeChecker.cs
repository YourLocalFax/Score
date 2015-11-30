using System;

namespace Score.Middle.TypeCheck
{
    using Front.Lex;
    using Front.Parse;
    using Front.Parse.SyntaxTree;

    using Symbols;

    /// <summary>
    /// Compiles entire programs.
    /// </summary>
    internal class ModTypeChecker : IAstVisitor
    {
        private readonly DetailLogger log;

        private SymbolTableWalker walker;

        public ModTypeChecker(DetailLogger log, SymbolTableWalker walker)
        {
            this.log = log;
            this.walker = walker;
        }

        public void Visit(Ast node)
        {
            node.children.ForEach(child => child.Accept(this));
        }

        public void Visit(NodeFnDecl fn)
        {
            var name = fn.Name;

            // The decl:

            if (fn.body != null)
            {
                walker.Step();

                var typeChecker = new FnTypeChecker(log, walker);
                fn.body.ForEach(node => node.Accept(typeChecker));

                // TODO(kai): check that returns are valid, too. Some can be done in the FnTypeChecker,
                // but the rest should be done here? I dunno yet.
            }
        }

        public void Visit(NodeTypeDef type)
        {
        }

        public void Visit(NodeLet let)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeId id)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeInt i)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeStr s)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeTuple tuple)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeIndex index)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeInvoke invoke)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeInfix infix)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeSuffix suffix)
        {
            throw new NotImplementedException();
        }
    }
}
