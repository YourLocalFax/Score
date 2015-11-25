using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse.SyntaxTree
{
    using Types;

    internal sealed class NodeFnDecl : Node
    {
        public Location start, end;
        public Mods mods;

        // SO, a function decl needs a name.
        public string name;
        public bool isNameOperator;
        public Span nameSpan;

        // Eventually, a function decl will need a target (maybe? inside classes/types/impl blocks)
        // TODO(kai): deal with this later, please.

        // We need paramters and a return type
        // This is a list of parameters:
        public List<ParamInfo> @params;

        public TypeInfo retType;
        public Span retSpan;

        internal override Span Span => start + end;

        public NodeFnDecl(Location start, Mods mods)
        {
            this.start = start;
            this.mods = mods;
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
