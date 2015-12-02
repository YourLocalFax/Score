
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Middle.TypeResolve
{
    using Front;
    using Front.Parse;
    using Front.Parse.SyntaxTree;
    using Front.Parse.Ty;

    using Symbols;

    internal sealed class TypeResolver : IAstVisitor
    {
        private readonly DetailLogger log;

        private SymbolTableWalker walker;

        private bool hasResolved;

        public TypeResolver(DetailLogger log)
        {
            this.log = log;
        }

        public void Resolve(Ast ast, SymbolTable symbols)
        {
            walker = new SymbolTableWalker(symbols);

            hasResolved = false;
            while (!hasResolved)
            {
                hasResolved = true;
                ast.Accept(this);
            }
        }

        private void ResolveTy(Spanned<TyRef> spTy)
        {
            var ty = spTy.value;
            if (ty is PathTyRef)
            {
                var path = ty as PathTyRef;
                if (!path.Resolved)
                {
                    // TODO(kai): eventually this will all be qualified, we'll have to deal with that.
                    var name = path.name;
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
                            path.Resolve(symTy);
                        else hasResolved = false;
                    }
                    else path.Resolve(symTy);
                }
            }
        }

        public void Visit(Ast node)
        {
            node.children.ForEach(child => child.Accept(this));
        }

        public void Visit(NodeFnDecl fn)
        {
            fn.Parameters.ForEach(param => ResolveTy(param.spTy));
            ResolveTy(fn.ReturnParameter.spTy);
            if (fn.body != null)
            {
                walker.Step();
                fn.body.ForEach(node => node.Accept(this));
            }
        }

        public void Visit(NodeTypeDef type)
        {
            ResolveTy(type.spTy);
        }

        public void Visit(NodeLet let)
        {
            if (let.binding.IsTyd)
                ResolveTy(let.binding.spTy);
            let.value.Accept(this);
        }

        public void Visit(NodeId id)
        {
        }

        public void Visit(NodeBool b)
        {
        }

        public void Visit(NodeInt i)
        {
            // TODO(kai): resolve suffixes? prob doesn't matter yet.
        }

        public void Visit(NodeStr s)
        {
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

        public void Visit(NodeIf @if)
        {
            @if.conditions.ForEach(cond =>
            {
                cond.condition.Accept(this);
                walker.Step();
                cond.body.ForEach(node => node.Accept(this));
            });
            if (@if.fail.Count > 0)
            {
                walker.Step();
                @if.fail.ForEach(node => node.Accept(this));
            }
        }
    }
}
