using System;

using Log;
using Symbols;
using SyntaxTree;
using SyntaxTree.Data;

using LLVMSharp;
using static LLVMSharp.LLVM;

namespace CodeGen
{
    internal sealed class ModCompiler : IAstVisitor
    {
        private readonly DetailLogger log;
        private readonly SymbolTableWalker walker;
        private readonly LLVMContextRef context;
        private readonly LLVMModuleRef module;
        private readonly LLVMBuilderRef builder;

        public ModCompiler(DetailLogger log, SymbolTableWalker walker,
            LLVMContextRef context, LLVMModuleRef module, LLVMBuilderRef builder)
        {
            this.log = log;
            this.walker = walker;
            this.context = context;
            this.module = module;
            this.builder = builder;
        }

        public void Visit(Ast node) =>
            node.children.ForEach(child => child.Accept(this));

        public void Visit(NodeFnDecl fn)
        {
            var self = AddFunction(module, fn.Name, TypeConverter.ToLLVMTy(fn.ty.Raw, context));
            for (var i = 0u; i < fn.parameters.Count; i++)
            {
                var param = fn.parameters[(int)i];
                if (param.HasName)
                    SetValueName(GetParam(self, i), param.Name + ".p");
            }
            if (fn.header.modifiers.Has(ModifierType.EXTERN))
                SetLinkage(self, LLVMLinkage.LLVMExternalLinkage);
            if (fn.body != null)
            {
                walker.StepIn();
                PositionBuilderAtEnd(builder, AppendBasicBlock(self, ".entry"));
                for (var i = 0u; i < fn.parameters.Count; i++)
                {
                    var param = fn.parameters[(int)i];

                    var sym = walker.Current.Lookup(param.Name);
                    sym.userdata = BuildAlloca(builder, TypeConverter.ToLLVMTy(param.Ty.Raw, context), param.Name);

                    BuildStore(builder, GetParam(self, i), (LLVMValueRef)sym.userdata);
                }
                var compiler = new FnCompiler(log, walker, context, module, builder, self);
                fn.body.ForEach(node => node.Accept(compiler));
                walker.StepOut();
                BuildRetVoid(builder);
            }
        }

        public void Visit(NodeTypeDef typeDef)
        {
        }

        public void Visit(NodeInvoke invoke)
        {
            throw new NotImplementedException();
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

        public void Visit(NodeEnclosed enc)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeIndex index)
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

        public void Visit(NodeTuple tuple)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeStr s)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeBool b)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeRet ret)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeIf @if)
        {
            throw new NotImplementedException();
        }
    }
}
