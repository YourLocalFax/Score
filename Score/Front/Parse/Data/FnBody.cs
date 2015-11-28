using System;
using System.Collections;
using System.Collections.Generic;

namespace Score.Front.Parse.Data
{
    using Lex;
    using SyntaxTree;

    internal sealed class FnBody : IEnumerable<Node>
    {
        public Token eq;
        public Token lbrace;
        private readonly List<Node> body = new List<Node>();
        public Token rbrace;

        public int Count => body.Count;
        public Node this[int index] => body[index];

        public void Add(Node node) => body.Add(node);
        public void ForEach(Action<Node> action) => body.ForEach(action);

        public IEnumerator<Node> GetEnumerator() => body.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => body.GetEnumerator();
    }
}
