using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LLVMSharp;

using static LLVMSharp.LLVM;

namespace Score.Back.LLVM
{
    using Front.Parse;
    using Front.Parse.SyntaxTree;
    using Front.Parse.Types;

    using Middle.Symbols;

    using Types;

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

        public void Visit(NodeFnDecl fnDecl)
        {
            // Do we need this? Probably not
            var sym = walker.Current.Lookup(fnDecl.name);

            var paramTypes = new LLVMTypeRef[fnDecl.@params.Count];
            for (int i = 0; i < paramTypes.Length; i++)
            {
                var paramInfo = fnDecl.@params[i];

                // TEMP CODE PLZ FIXME(kai): this is bad

                var type = ScoreType.TempGetType(log, paramInfo.type);
                var llvmType = type.TempGetLLVMType(Context);

                paramTypes[i] = llvmType;
            }

            var returnType = ScoreType.TempGetType(log, fnDecl.retType).TempGetLLVMType(Context);
            var fnType = FunctionType(returnType, paramTypes, false);

            var fn = AddFunction(module, fnDecl.name, fnType);
            if (fnDecl.mods.Extern)
                SetLinkage(fn, LLVMLinkage.LLVMExternalLinkage);
        }

        public void Visit(NodeFn fn)
        {
            fn.decl.Accept(this);
            var self = GetNamedFunction(module, fn.decl.name);
            var sym = walker.Current.Lookup(fn.decl.name);

            walker.Step();

            var compiler = new FnCompiler(manager, module, self);
            fn.body.ForEach(node => node.Accept(compiler));

            if (fn.decl.retType == TypeInfo.VOID)
                BuildRetVoid(compiler.builder);
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
