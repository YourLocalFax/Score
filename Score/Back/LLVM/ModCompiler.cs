using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Back.LLVM
{
    using Front.Parse;
    using Front.Parse.SyntaxTree;

    /// <summary>
    /// Compiles entire programs.
    /// </summary>
    internal sealed class ModCompiler : IAstVisitor
    {
        public void Visit(NodeFnDecl fnDecl)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeFn fn)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeInt i)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeStr s)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeTuple tuple)
        {
            throw new NotImplementedException();
        }

        public void Visit(NodeInvoke invoke)
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
