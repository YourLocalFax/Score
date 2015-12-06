using System.IO;

using SyntaxTree;
using SyntaxTree.Data;
using Util;

namespace Dbg
{
    public sealed class AstWriter : IAstVisitor
    {
        private readonly TextWriter writer;

        private int tabs = 0;

        public AstWriter(TextWriter writer)
        {
            this.writer = writer;
        }

        // TODO(kai): possible error checking on this, please
        private void Tab() => tabs++;
        private void Untab() => tabs--;

        private void WriteTabs() => Write("     ".Repeat(tabs));

        private void Write(string message) =>
            writer.Write(message);

        private void Write(string format, params object[] args) =>
            writer.Write(string.Format(format, args));

        private void WriteLine(string message) =>
            writer.WriteLine(message);

        private void WriteLine(string format, params object[] args) =>
            writer.WriteLine(string.Format(format, args));

        private void WriteLine() => writer.WriteLine();

        public void Visit(Ast node)
        {
            node.children.ForEach(child =>
            {
                WriteTabs();
                child.Accept(this);
                WriteLine();
            });
        }

        private void WriteMods(Modifiers mods)
        {
            mods.modifiers.ForEach(mod => Write("'{0}'{1} ",
                mod.token.Image, mod.optArg != null ? string.Format(" \"{0}\"", mod.optArg.value) : ""));
        }

        private void WriteParameter(Parameter param)
        {
            if (param.name != null)
                Write(param.name.value);
            if (param.name != null && param.HasTy)
                Write(" ':' ");
            if (param.HasTy)
                Write(param.Ty.ToString());
        }

        public void Visit(NodeFnDecl fn)
        {
            // the decl
            WriteMods(fn.header.modifiers);
            // 'fn' name '|' args ':' type ',' '|' '->' type
            Write("'fn' ");
            // TODO(kai): write this better <3
            Write(fn.Name);
            Write(" '|' ");
            var p = fn.parameters;
            for (int i = 0; i < p.Count; i++)
            {
                if (i > 0)
                    Write(" ',' ");
                WriteParameter(p[i]);
            }
            Write(" '|' ");
            // TODO(kai): change this when you have the infer type
            if (fn.@return != null)
            {
                Write("'->' ");
                if (fn.@return.Ty.IsVoid)
                    Write("'()'");
                else WriteParameter(fn.@return);
            }
            WriteLine();

            // the body
            if (fn.body != null)
            {
                WriteTabs();
                if (fn.body.eq != null)
                    Write("'=' ");
                Write("'{'");
                Tab();
                WriteLine();
                fn.body.ForEach(node => {
                    WriteTabs();
                    node.Accept(this);
                    WriteLine();
                });
                Untab();
                WriteTabs();
                Write("'}'");
            }
        }

        public void Visit(NodeTypeDef type)
        {
            Write("'type' ");
            Write(type.name.value);
            Write(" '=' ");
            Write(type.Ty.ToString());
        }

        public void Visit(NodeLet let)
        {
            Write("'let' ");
            WriteParameter(let.binding);
            Write(" '=' ");
            let.value.Accept(this);
        }

        public void Visit(NodeId id)
        {
            Write(id.Image);
        }

        public void Visit(NodeBool b)
        {
            Write(b.Value ? "true" : "false");
        }

        public void Visit(NodeInt i)
        {
            Write("{0}{1}", i.Token.Image, i.Token.NumericSuffix);
        }

        public void Visit(NodeStr s)
        {
            Write("{0}{1}\"{2}\"", s.Token.StrC ? "c" : "", s.Token.StrVerbatim ? "v" : "", s.Value);
        }

        public void Visit(NodeEnclosed enclosed)
        {
            Write("'(' ");
            enclosed.expr.Accept(this);
            Write(" ')'");
        }

        public void Visit(NodeTuple tuple)
        {
            Write("'(' ");
            foreach (var value in tuple.values)
            {
                value.Accept(this);
                Write(" ',' ");
            }
            Write("')'");
        }

        public void Visit(NodeIndex index)
        {
            Write("'(' ");
            index.target.Accept(this);
            Write(" ')'");
            Write(" '.' " + index.index.Image);
        }

        public void Visit(NodeInvoke invoke)
        {
            invoke.target.Accept(this);
            foreach (var arg in invoke.args)
            {
                Write(" ");
                arg.Accept(this);
            }
        }

        public void Visit(NodeInfix infix)
        {
            var image = infix.op.Image;
            Write("'(' ");
            infix.left.Accept(this);
            Write(" ')'");
            // TODO(kai): maybe use something different
            if (Lex.Util.IsIdentifierStart(image[0]))
                Write(" `" + image + " ");
            else Write(" " + image + " ");
            Write("'(' ");
            infix.right.Accept(this);
            Write(" ')'");
        }

        public void Visit(NodeSuffix suffix)
        {
            var image = suffix.op.Image;
            Write("'(' ");
            suffix.target.Accept(this);
            Write(" ')'");
            // TODO(kai): maybe use something different
            if (Lex.Util.IsIdentifierStart(image[0]))
                Write(" `" + image + " ");
            else Write(" " + image + " ");
        }

        public void Visit(NodeIf @if)
        {
            @if.conditions.ForEach(cond =>
            {
                Write("'if' ");
                cond.condition.Accept(this);
                WriteLine(" '{' ");
                Tab();
                cond.body.ForEach(node =>
                {
                    WriteTabs();
                    node.Accept(this);
                    WriteLine();
                });
                Untab();
                WriteLine();
                WriteTabs();
                Write(" '}' ");
            });
            if (@if.fail.Count > 0)
            {
                Write("'el' ");
                Write(" '{' ");
                Tab();
                @if.fail.ForEach(node =>
                {
                    WriteTabs();
                    node.Accept(this);
                    WriteLine();
                });
                Untab();
                WriteLine();
                WriteTabs();
                Write(" '}' ");
            }
        }
    }
}
