using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Score.Front.Parse
{
    internal sealed class IdentPath
    {
        public readonly List<string> path;

        public IdentPath()
        {
            path = new List<string>();
        }

        public void Add(string ident) => path.Add(ident);
        // TODO(kai): we might not need this, but it's here just in case.
        public void ForEach(Action<string> action) => path.ForEach(action);

        public override string ToString()
        {
            var builder = new StringBuilder();

            // TODO(kai): probably a better way to do this, never thought about it.
            for (int i = 0; i < path.Count; i++)
            {
                if (i > 0)
                    builder.Append(".");
                builder.Append(path[i]);
            }

            return builder.ToString();
        }
    }
}
