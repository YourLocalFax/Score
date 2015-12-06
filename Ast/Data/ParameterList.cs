using System;
using System.Collections;
using System.Collections.Generic;

namespace SyntaxTree.Data
{
    public sealed class ParameterList : IEnumerable<Parameter>
    {
        // TODO(kai): maybe make this an array?
        private readonly List<Parameter> parameters = new List<Parameter>();

        public int Count => parameters.Count;
        public Parameter this[int index] => parameters[index];

        public void Add(Parameter parameter) => parameters.Add(parameter);
        public void ForEach(Action<Parameter> action) => parameters.ForEach(action);

        public IEnumerator<Parameter> GetEnumerator() => parameters.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => parameters.GetEnumerator();
    }
}
