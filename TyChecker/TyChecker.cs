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
        private SymbolTableWalker walker;

        private readonly Stack<TyRef> stack = new Stack<TyRef>();

        public TyChecker(DetailLogger log)
        {
            this.log = log;
        }

        private void Push(TyRef ty) => stack.Push(ty);

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

        public void Visit(NodeInvoke invoke)
        {
            invoke.args.ForEach(arg => arg.Accept(this));
            var invokeArgs = PopCount(invoke.args.Count);

            var name = invoke.TargetName;

            var sym = walker.Current.Lookup(name);
            if (sym == null)
            {
                var builder = new StringBuilder().Append('|');

                for (int i = 0; i < invokeArgs.Length; i++)
                {
                    if (i > 0)
                        builder.Append(", ");
                    builder.Append(invokeArgs[i]);
                }

                builder.Append('|');

                // TODO(kai): rephrase this?
                log.Error(invoke.spName.span, "No method \"{0}\" with parameter types {1} exists.", name, builder.ToString());
            }
        }

        public void Visit(NodeLet let)
        {
            let.value.Accept(this);
            var valueTy = Pop();

            if (let.binding.InferTy)
                let.binding.spTy = valueTy.Spanned();
            else if (let.binding.Ty != valueTy)
                log.Error(let.binding.spTy.span, "Type mismatch: Cannot assign {0} to {1}.", valueTy, let.binding.Ty);
        }

        public void Visit(NodeId id)
        {
        }

        public void Visit(NodeInt i)
        {
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

        public void Visit(NodeIf @if)
        {
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
        }

        public void Visit(NodeBool b)
        {
        }
    }
}
