using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score.Middle.FnPopulation
{
    using Back;

    using Front;
    using Front.Lex;
    using Front.Parse;
    using Front.Parse.SyntaxTree;
    using Front.Parse.Ty;

    using Symbols;

    internal sealed class FnPopulator : IAstVisitor
    {
        private readonly DetailLogger log;
        private readonly GlobalStateManager manager;
        private readonly LLVMModuleRef module;

        private SymbolTableWalker walker;

        private bool hasResolved;

        public FnPopulator(DetailLogger log, GlobalStateManager manager, LLVMModuleRef module)
        {
            this.log = log;
            this.manager = manager;
            this.module = module;
        }

        public void Populate(Ast ast, SymbolTable symbols)
        {
            walker = new SymbolTableWalker(symbols);
            ast.Accept(this);
        }

        public void Visit(Ast node)
        {
            node.children.ForEach(child => child.Accept(this));
        }

        public void Visit(NodeFnDecl fn)
        {
            var sym = walker.Current.Lookup(fn.Name);

            var llvmTy = fn.ty.GetLLVMTy(manager.context);
            var llvmFn = AddFunction(module, fn.Name, llvmTy);

            if (fn.header.modifiers.Has(Token.Type.EXTERN))
                SetLinkage(llvmFn, LLVMLinkage.LLVMExternalLinkage);
            
            (sym as FnSymbol).llvmFn = llvmFn;

            if (fn.body != null)
            {
                walker.Step();
                fn.body.ForEach(node => node.Accept(this));
            }
        }

        public void Visit(NodeTypeDef type)
        {
        }

        public void Visit(NodeLet let)
        {
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
    }
}
