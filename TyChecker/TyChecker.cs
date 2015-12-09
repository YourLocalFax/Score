using System;
using System.Collections.Generic;
using System.Text;

using Log;
using Source;
using Symbols;
using SyntaxTree;
using Ty;

namespace TyChecker
{
    internal sealed class TyChecker : IAstVisitor
    {
        private readonly DetailLogger log;
        private readonly SymbolTableWalker walker;

        private readonly Stack<TyRef> stack = new Stack<TyRef>();

        public TyChecker(DetailLogger log, SymbolTableWalker walker)
        {
            this.log = log;
            this.walker = walker;
        }

        private void Push(TyRef ty) => stack.Push(ty.Raw);

        private TyRef Pop() => stack.Pop();

        private TyRef[] PopCount(int count)
        {
            if (count > stack.Count)
                throw new ArgumentException(string.Format("Cannot pop {0} values from the stack, only {1} values are present.",
                    count, stack.Count), "count");
            var result = new TyRef[count];
            for (int i = count; i-- > 0;)
                result[i] = stack.Pop();
            return result;
        }

        public void Visit(Ast node) =>
            node.children.ForEach(child => child.Accept(this));

        public void Visit(NodeFnDecl fn)
        {
            if (fn.body != null)
            {
                walker.StepIn();
                fn.body.ForEach(node => node.Accept(this));

                // TODO(kai): check that returns are valid, too. Some can be done in the FnTypeChecker,
                // but the rest should be done here? I dunno yet.

                walker.StepOut();
            }
        }

        public void Visit(NodeTypeDef typeDef)
        {
        }

        private void ErrNoSuchFunction(Spanned<string> spName, TyRef[] args)
        {
            var builder = new StringBuilder().Append('|');

            for (int i = 0; i < args.Length; i++)
            {
                if (i > 0)
                    builder.Append(", ");
                builder.Append(args[i]);
            }

            builder.Append('|');

            log.Error(spName.span, "No method \"{0}\" with parameter types {1} exists.", spName.value, builder.ToString());
        }

        public void Visit(NodeInvoke invoke)
        {
            invoke.args.ForEach(arg => arg.Accept(this));
            var invokeArgs = PopCount(invoke.args.Count);

            var name = invoke.TargetName;

            var symTy = walker.Current.Lookup(name)?.Ty as FnTyRef;
            if (symTy == null)
            {
                log.Error(invoke.spName.span, "No function \"{0}\" was found.", name);
                Push(TyVoid.VoidTy);
                return;
            }

            Push(symTy.returnTy);

            if (invokeArgs.Length != symTy.parameterTys.Count)
            {
                ErrNoSuchFunction(invoke.spName, invokeArgs);
                return;
            }

            for (int i = 0, len = invokeArgs.Length; i < len; i++)
            {
                if (symTy.parameterTys[i].Raw != invokeArgs[i])
                {
                    ErrNoSuchFunction(invoke.spName, invokeArgs);
                    break;
                }
            }
        }

        public void Visit(NodeLet let)
        {
            let.value.Accept(this);
            var valueTy = Pop();

            if (let.Ty is InferTyRef)
            {
                let.spTy = valueTy.Spanned();
                (walker.Current.Lookup(let.Name) as VarSymbol).ty = valueTy;
            }

            if (let.Ty.Raw != valueTy)
                log.Error(let.spTy.span, "Type mismatch: Cannot assign {0} to {1}.", valueTy, let.Ty);
        }

        public void Visit(NodeId id)
        {
            var sym = walker.Current.Lookup(id.Image);
            Push(sym.Ty);
        }

        public void Visit(NodeInt i)
        {
            var token = i.Token;
            var suffix = token.NumericSuffix;

            TyRef ty;
            switch (suffix)
            {
                case "i8":  ty = TyInt.Int8Ty; break;
                case "i16": ty = TyInt.Int16Ty; break;
                case "":
                case "i32": ty = TyInt.Int32Ty; break;
                case "i64": ty = TyInt.Int64Ty; break;
                case "u8":  ty = TyUint.Uint8Ty; break;
                case "u16": ty = TyUint.Uint16Ty; break;
                case "u32": ty = TyUint.Uint32Ty; break;
                case "u64": ty = TyUint.Uint64Ty; break;

                default:
                    ty = TyInt.Int32Ty;
                    log.Error(i.Span, "Invalid integer suffix \"{0}\"!", suffix);
                    break;
            }

            Push(ty);
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
            Push(new PointerTyRef(TyInt.Int8Ty, false));
        }

        public void Visit(NodeBool b)
        {
            Push(TyBool.BoolTy);
        }

        public void Visit(NodeRet ret)
        {
        }

        public void Visit(NodeIf @if)
        {
        }
    }
}
