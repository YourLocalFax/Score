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

    /// <summary>
    /// Compiles entire programs.
    /// </summary>
    internal sealed class FnCompiler : IAstVisitor
    {
        private readonly GlobalStateManager manager;
        private LLVMContextRef Context => manager.context;
        private readonly LLVMModuleRef module;
        private readonly LLVMBuilderRef builder;

        // FIXME(kai): wrap these in our own type, to keep score type information.
        private readonly Stack<LLVMValueRef> stack = new Stack<LLVMValueRef>();

        public FnCompiler(GlobalStateManager manager, LLVMModuleRef module)
        {
            this.manager = manager;
            this.module = module;
            builder = CreateBuilderInContext(Context);
        }

        public void Begin()
        {
        }

        public void End()
        {
        }

        private LLVMValueRef[] PopCount(int count)
        {
            if (count > stack.Count)
                throw new ArgumentException(string.Format("Cannot pop {0} values from the stack, only {1} values are present.",
                    count, stack.Count), "count");
            var result = new LLVMValueRef[count];
            for (int i = count; i-- >= 0;)
                result[i] = stack.Pop();
            return result;
        }

        public void Visit(NodeFnDecl fnDecl)
        {
        }

        public void Visit(NodeFn fn)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeInvoke invoke)
        {
            // get the target
            // load the args
            // check types, do any implicit casts.
            // call the thing

            // puts c"Hello, world!"

            // look up puts
            // load c"Hello, world!"
            invoke.args.ForEach(arg => arg.Accept(this));
            // retrieve the values from the stack
            var argValues = PopCount(invoke.args.Count);
            // check that there is indeed one arg
            // check that the c-str is ^i8 (it is) because puts takes a ^i8
            // create the invocation
        }

        public void Visit(NodeInt i)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeStr s)
        {
            var value = s.str.value;
            // TODO(kai): assign names? something like sconst0 etc.?
            var constCStr = manager.CStrConst(builder, value);
            stack.Push(constCStr);
        }

        public void Visit(NodeTuple tuple)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeSuffix suffix)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeInfix infix)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeIndex index)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeId id)
        {
            throw new NotImplementedException();
        }

        public void Visit(Ast node)
        {
            throw new NotImplementedException();
        }
    }
}
