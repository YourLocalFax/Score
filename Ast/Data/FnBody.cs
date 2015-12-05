using System;
using System.Collections;
using System.Collections.Generic;

using Lex;
using Source;

namespace Ast.Data
{
    public sealed class FnBody : IEnumerable<Spanned<Node>>
    {
        public Spanned<Token> eq, lbrace, rbrace;
        private readonly List<Spanned<Node>> body = new List<Spanned<Node>>();

        public Span Span
        {
            get
            {
                // NOTE(kai): If there's not '=', there must be '{'
                var startSpan = eq != null ? eq.span : lbrace.span;
                // NOTE(kai): if there's no '}', there must be one body node.
                var endSpan = rbrace != null ? rbrace.span : body[0].span;
                return new Span(startSpan.fileName, startSpan.start, endSpan.end);
            }
        }

        public int Count => body.Count;
        public Spanned<Node> this[int index] => body[index];

        public void Add(Spanned<Node> node) => body.Add(node);
        public void ForEach(Action<Spanned<Node>> action) => body.ForEach(action);

        public IEnumerator<Spanned<Node>> GetEnumerator() => body.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => body.GetEnumerator();
    }
}
