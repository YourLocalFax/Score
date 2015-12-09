using System;
using System.Collections.Generic;

using Ext;
using Log;
using Source;
using Symbols;
using SyntaxTree;
using Ty;

using LLVMSharp;
using static LLVMSharp.LLVM;

namespace CodeGen
{
    internal sealed class FnCompiler : IAstVisitor
    {
        private readonly DetailLogger log;
        private readonly SymbolTableWalker walker;
        private readonly LLVMContextRef context;
        private readonly LLVMModuleRef module;
        private readonly LLVMBuilderRef builder;
        private readonly LLVMValueRef self;

        private readonly Stack<ScoreVal> stack = new Stack<ScoreVal>();

        public FnCompiler(DetailLogger log, SymbolTableWalker walker,
            LLVMContextRef context, LLVMModuleRef module, LLVMBuilderRef builder, LLVMValueRef self)
        {
            this.log = log;
            this.walker = walker;
            this.context = context;
            this.module = module;
            this.builder = builder;
            this.self = self;
        }

        private ScoreVal Push(Span span, TyRef ty, LLVMValueRef value)
        {
            var val = new ScoreVal(span, ty, value);
            stack.Push(val);
            return val;
        }

        private ScoreVal Pop() => stack.Pop();

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

        public void Visit(Ast node) =>
            node.children.ForEach(child => child.Accept(this));

        public void Visit(NodeFnDecl fn)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeTypeDef typeDef)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeInvoke invoke)
        {
            var name = invoke.TargetName;
            invoke.args.ForEach(arg => arg.Accept(this));
            var args = PopCount(invoke.args.Count);
            var result = BuildCall(builder, GetNamedFunction(module, name), args.Select(arg => arg.value), "");
            Push(invoke.Span, (walker.Current.Lookup(invoke.TargetName).Ty as FnTyRef).returnTy, result);
        }

        public void Visit(NodeLet let)
        {
            var sym = walker.Current.Lookup(let.Name);
            sym.userdata = BuildAlloca(builder, TypeConverter.ToLLVMTy(let.Ty.Raw, context), let.Name);

            let.value.Accept(this);
            var value = Pop().value;

            BuildStore(builder, value, (LLVMValueRef)sym.userdata);
        }

        public void Visit(NodeId id)
        {
            var name = id.Image;
            var sym = walker.Current.Lookup(name);
            var val = BuildLoad(builder, (LLVMValueRef)sym.userdata, "");
            Push(id.Span, sym.Ty, val);
        }

        public void Visit(NodeInt i)
        {
            var token = i.Token;
            var suffix = token.NumericSuffix;

            TyRef ty;
            switch (suffix)
            {
                case "i8": ty = TyInt.Int8Ty; break;
                case "i16": ty = TyInt.Int16Ty; break;
                case "":
                case "i32": ty = TyInt.Int32Ty; break;
                case "i64": ty = TyInt.Int64Ty; break;
                case "u8": ty = TyUint.Uint8Ty; break;
                case "u16": ty = TyUint.Uint16Ty; break;
                case "u32": ty = TyUint.Uint32Ty; break;
                case "u64": ty = TyUint.Uint64Ty; break;

                default:
                    ty = TyInt.Int32Ty;
                    log.Error(i.Span, "Invalid integer suffix \"{0}\"!", suffix);
                    break;
            }

            Push(i.Span, ty, ConstInt(TypeConverter.ToLLVMTy(ty.Raw, context), i.Token.IntegerValue, ty is TyUint));
        }

        public void Visit(NodeEnclosed enc)
        {
            enc.expr.Accept(this);
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
            var value = s.Value;
            var constCStr = BuildGlobalStringPtr(builder, value, "");
            Push(s.Span, new PointerTyRef(TyInt.Int8Ty, false), constCStr);
        }

        public void Visit(NodeBool b)
        {
            Push(b.Span, TyBool.BoolTy, ConstInt(Int1TypeInContext(context), b.Value ? 1ul : 0ul, false));
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
