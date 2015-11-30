﻿using System;
using System.Collections.Generic;

using LLVMSharp;

using static LLVMSharp.LLVM;

namespace Score.Back
{
    using Val;

    using Front;
    using Front.Parse;
    using Front.Parse.SyntaxTree;
    using Front.Parse.Ty;

    using Middle.Symbols;

    /// <summary>
    /// Compiles entire programs.
    /// </summary>
    internal sealed class FnCompiler : IAstVisitor
    {
        private readonly DetailLogger log;
        private readonly GlobalStateManager manager;
        private LLVMContextRef Context => manager.context;
        private readonly LLVMModuleRef module;
        internal readonly LLVMBuilderRef builder;
        private readonly SymbolTableWalker walker;

        private readonly ScoreVal self;

        // FIXME(kai): wrap these in our own type, to keep score type information.
        private readonly Stack<ScoreVal> stack = new Stack<ScoreVal>();

        public FnCompiler(DetailLogger log, GlobalStateManager manager, LLVMModuleRef module, SymbolTableWalker walker, ScoreVal self)
        {
            this.log = log;
            this.manager = manager;
            this.module = module;
            this.walker = walker;
            this.self = self;

            builder = CreateBuilderInContext(Context);
            PositionBuilderAtEnd(builder, AppendBasicBlockInContext(Context, self.value, ".entry"));
        }

        private ScoreVal Push(Span span, TyRef ty, LLVMValueRef value)
        {
            var val = new ScoreVal(span, ty, value);
            stack.Push(val);
            return val;
        }

        private ScoreVal[] PopCount(int count)
        {
            if (count > stack.Count)
                throw new ArgumentException(string.Format("Cannot pop {0} values from the stack, only {1} values are present.",
                    count, stack.Count), "count");
            var result = new ScoreVal[count];
            for (int i = count; i-- > 0;)
                result[i] = stack.Pop();
            return result;
        }

        public void Visit(NodeFnDecl fn)
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

            // TODO(kai): Check types in a new (type checker) pass? yes.
            // look up puts
            // TODO(kai): THIS IS BAD PLS STOP
            // TODO(kai): mangle names
            // TODO(kai): with name mangling comes searching for the specific type we're looking for,
            // so don't forget about that.
            var name = (invoke.target as NodeId).token.Image;
            var sym = walker.Current.Lookup(name);
            if (sym == null)
            {
                // ERRORRRRR
                return; // TODO(kai): maybe we shouldn't return here,
                // There are aguments that haven't been processed yet. that might be bad?
            }
            var ty = sym.ty as TyFn;
            var fn = GetNamedFunction(module, name);
            // load c"Hello, world!"
            invoke.args.ForEach(arg => arg.Accept(this));
            // retrieve the values from the stack
            var argValues = PopCount(invoke.args.Count);
            // check that there is indeed one arg
            // check that the c-str is ^i8 (it is) because puts takes a ^i8
            if (ty.parameters.Count != argValues.Length)
            {
                // ERRRORRRRRRRR
                return;
            }
            for (int i = 0, len = argValues.Length; i < len; i++)
            {
                var paramTy = ty.parameters[i].ty;
                var argTy = argValues[i].ty;
                if (!paramTy.SameAs(argTy))
                {
                    // TODO(kai): MUCH more description, please <3
                    log.Error(argValues[i].span, "Argument {0}'s type ({1}) does not match parameter type ({2}).",
                        i, argTy, paramTy);
                    return;
                }
            }
            // create the invocation
            var args = argValues.Map(val => val.value);
            BuildCall(builder, fn, args, "");
        }

        public void Visit(NodeInt i)
        {
            // TODO(kai): things need types, please.
            var value = i.token.n;

            // FIXME(kai): This is temp just to get the type of the int.
            // Since ints are allowed to be literals for any type, that needs to be considered later.

            TyRef intTy = TyRef.Int32Ty;
            if (i.token.suffix.Length > 0)
            {
                switch (i.token.suffix)
                {
                    default:
                        log.Error(i.Span, "TEMP suffixes are not supported as of yet.");
                        break;
                }
            }

            var constInt = ConstInt(intTy.GetLLVMTy(Context), value, false);
            Push(i.Span, intTy, constInt);
        }

        public void Visit(NodeStr s)
        {
            // TODO(kai): things need types, please.
            var value = s.str.value;
            // TODO(kai): Not all strings are C-Strings...
            // TODO(kai): assign names? something like sconst0 etc.?
            var constCStr = manager.CStrConst(builder, value);
            Push(s.Span, TyRef.PointerTo(TyRef.Int8Ty, false), constCStr);
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