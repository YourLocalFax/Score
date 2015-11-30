using System;
using System.Collections.Generic;
using System.Text;

namespace Score.Middle.TypeCheck
{
    using Front.Parse;
    using Front.Parse.Data;
    using Front.Parse.SyntaxTree;
    using Front.Parse.Ty;

    using Symbols;

    /// <summary>
    /// Compiles entire programs.
    /// </summary>
    internal sealed class FnTypeChecker : IAstVisitor
    {
        private readonly DetailLogger log;
        private readonly SymbolTableWalker walker;

        private readonly Stack<TyRef> stack = new Stack<TyRef>();

        public FnTypeChecker(DetailLogger log, SymbolTableWalker walker)
        {
            this.log = log;
            this.walker = walker;
        }

        private void Push(TyRef ty) => stack.Push(ty);

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

        public void Visit(NodeFnDecl fn)
        {
        }

        public void Visit(NodeTypeDef type)
        {
        }

        public void Visit(NodeInvoke invoke)
        {
            // TODO(kai): I think what I'll do is have the symbol table
            // store the name as it appears in Score code along side
            // the type of the value, so when looking up functions
            // I can specify what type I'm looking for. If that fails,
            // then there's not function with the specified type.
            //
            // This will fail eventually, so it's not the only way that
            // it should be handled. When implicit conversions become a
            // thing those should be handled, and varargs might not behave
            // properly. Neither of these things are required for the language,
            // though, so they might can wait until the second compiler that
            // gets written in Score itself.

            invoke.args.ForEach(arg => arg.Accept(this));
            var invokeArgs = PopCount(invoke.args.Count);

            var name = (invoke.target as NodeId).token.Image;

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
                log.Error(invoke.target.Span, "No method \"{0}\" with parameter types {1} exists.", name, builder.ToString());
            }
        }

        public void Visit(NodeInt i)
        {
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

            Push(intTy);
        }

        public void Visit(NodeStr s)
        {
            Push(TyRef.PointerTo(TyRef.Int8Ty, false));
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
