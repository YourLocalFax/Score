﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Score.Front.Parse.Data
{
    internal sealed class QualifiedName
    {
        public readonly List<NameOrOp> names;

        public QualifiedName()
        {
            names = new List<NameOrOp>();
        }

        public void Add(NameOrOp name) => names.Add(name);
        public void Add(Name id) => names.Add(NameOrOp.FromId(id));
        public void Add(Op op) => names.Add(NameOrOp.FromOp(op));
        // TODO(kai): we might not need this, but it's here just in case.
        public void ForEach(Action<NameOrOp> action) => names.ForEach(action);

        public override string ToString()
        {
            var builder = new StringBuilder();

            // TODO(kai): probably a better way to do this, never thought about it.
            for (int i = 0; i < names.Count; i++)
            {
                if (i > 0)
                    builder.Append(".");
                builder.Append(names[i]);
            }

            return builder.ToString();
        }
    }
}
