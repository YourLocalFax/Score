using System;

using LLVMSharp;
using static LLVMSharp.LLVM;

namespace Score.Back
{
    using Val;

    using Front.Lex;
    using Front.Parse;
    using Front.Parse.SyntaxTree;

    using Middle.Symbols;

    /// <summary>
    /// Compiles entire programs.
    /// </summary>
    internal class ModCompiler : IAstVisitor
    {
        private readonly DetailLogger log;

        private readonly GlobalStateManager manager;
        private LLVMContextRef Context => manager.context;
        private readonly LLVMModuleRef module;
        private SymbolTableWalker walker;

        public ModCompiler(DetailLogger log, GlobalStateManager manager, LLVMModuleRef module, SymbolTableWalker walker)
        {
            this.log = log;
            this.manager = manager;
            this.module = module;
            this.walker = walker;
        }

        public void Visit(Ast node)
        {
            node.children.ForEach(child => child.Accept(this));
        }

        /*
        unsafe fn get_puts_fn(c: ContextRef, m: ModuleRef) -> ValueRef {
	        let param_types: &[TypeRef; 1] = &[ LLVMPointerType(LLVMInt8TypeInContext(c), 0) ];
	        let return_type = LLVMInt32TypeInContext(c);
	        let fn_type = LLVMFunctionType(return_type, param_types.as_ptr(), 1 as c_uint, False);
	        let fn_res = LLVMAddFunction(m, c_str!("puts"), fn_type);
	        LLVMSetLinkage(fn_res, Linkage::ExternalLinkage as c_uint);
	        fn_res
        }
        */

        public void Visit(NodeFnDecl fn)
        {
            if (fn.body != null)
            {
                // TODO(kai): LookupFn(name)
                var sym = walker.Current.Lookup(fn.Name) as FnSymbol;

                walker.Step();

                var compiler = new FnCompiler(log, manager, module, walker, new ScoreVal(fn.Span, sym.ty, sym.llvmFn));
                fn.body.ForEach(node => node.Accept(compiler));

                if (fn.ReturnParameter.Ty.IsVoid)
                    BuildRetVoid(compiler.builder);
                // TODO(kai): ELSE RETURN OTHER THINGS PLZ
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
