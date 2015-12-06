using System;
using System.Collections;
using System.Collections.Generic;

using Lex;
using Source;

namespace Ast.Data
{
    public sealed class FnBody : IEnumerable<Node>
    {
        public Token eq, lbrace, rbrace;
        private readonly List<Node> body = new List<Node>();

        public Span Span
        {
            get
            {
                // NOTE(kai): If there's not '=', there must be '{'
                var startSpan = eq != null ? eq.span : lbrace.span;
                // NOTE(kai): if there's no '}', there must be one body node.
                var endSpan = rbrace != null ? rbrace.span : body[0].Span;
                return new Span(startSpan.fileName, startSpan.start, endSpan.end);
            }
        }

        public int Count => body.Count;
        public Node this[int index] => body[index];

        public void Add(Node node) => body.Add(node);
        public void ForEach(Action<Node> action) => body.ForEach(action);

        public IEnumerator<Node> GetEnumerator() => body.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => body.GetEnumerator();
    }
}
