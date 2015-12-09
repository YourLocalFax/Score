using Log;
using Source;
using Symbols;
using SyntaxTree;
using Ty;

namespace TyChecker
{
    internal sealed class TyResolver : IAstVisitor
    {
        private readonly DetailLogger log;

        private SymbolTableWalker walker;
        private bool hasResolved = false;

        public TyResolver(DetailLogger log)
        {
            this.log = log;
        }

        public void Resolve(Ast ast, SymbolTable symbols)
        {
            walker = new SymbolTableWalker(symbols);
            
            do
            {
                walker.Reset();
                hasResolved = true;
                ast.Accept(this);
            }
            while (!hasResolved);
        }

        private void ResolveTy(Spanned<TyRef> spTy)
        {
            var ty = spTy.value as PathTyRef;
            if (ty != null && !ty.Resolved)
            {
                if (!ty.Resolved)
                {
                    // TODO(kai): eventually this will all be qualified, we'll have to deal with that.
                    var name = ty.name;
                    var sym = walker.Current.Lookup(name);
                    if (sym == null)
                    {
                        // TODO(kai): use spanned types
                        log.Error(spTy.span, "Could not locate symbol \"{0}\", failed to resolve type.", name);
                        return;
                    }
                    var symTy = sym.Ty;
                    if (symTy is PathTyRef)
                    {
                        if ((symTy as PathTyRef).Resolved)
                            ty.Resolve(symTy);
                        else hasResolved = false;
                    }
                    else ty.Resolve(symTy);
                }
            }
        }

        public void Visit(Ast node) =>
            node.children.ForEach(child => child.Accept(this));

        public void Visit(NodeFnDecl fn)
        {
            fn.parameters.ForEach(param => ResolveTy(param.spTy));
            ResolveTy(fn.@return.spTy);
            if (fn.body != null)
            {
                walker.StepIn();
                fn.body.ForEach(node => node.Accept(this));
                walker.StepOut();
            }
        }

        public void Visit(NodeTypeDef typeDef)
        {
            ResolveTy(typeDef.spTy);
        }

        public void Visit(NodeInvoke invoke)
        {
            invoke.args.ForEach(arg => arg.Accept(this));
        }

        public void Visit(NodeLet let)
        {
            ResolveTy(let.spTy);
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
        }

        public void Visit(NodeInfix infix)
        {
            infix.left.Accept(this);
            infix.right.Accept(this);
        }

        public void Visit(NodeIf @if)
        {
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
    }
}
