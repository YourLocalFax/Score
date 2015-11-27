using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse.Data
{
    using Lex;
    using Ty;

    internal sealed class TyArgList
    {
        public readonly TokenOp lt;
        public readonly List<TyOrVoid> tyList = new List<TyOrVoid>();
        public readonly TokenOp gt;
    }
}
