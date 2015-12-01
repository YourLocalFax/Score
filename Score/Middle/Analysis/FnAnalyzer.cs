using System;

using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score.Middle.Analysis
{
    using Back;

    using Front.Parse;
    using Front.Parse.SyntaxTree;

    using Symbols;

    internal sealed class FnAnalyzer : IAstVisitor
    {
        private readonly DetailLogger log;
        private readonly SymbolTable symbols;

        private readonly GlobalStateManager manager;
        private readonly LLVMModuleRef module;

        public FnAnalyzer(DetailLogger log, SymbolTable symbols, GlobalStateManager manager, LLVMModuleRef module)
        {
            this.log = log;
            this.symbols = symbols;

            this.manager = manager;
            this.module = module;
        }

        public void Visit(Ast node)
        {
            // TODO(kai): this is not valid, error on it.
        }

        public void Visit(NodeFnDecl fn)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeTypeDef type)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeLet let)
        {
            symbols.InsertVar(let.binding.name.Image, let.binding.Ty);
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
