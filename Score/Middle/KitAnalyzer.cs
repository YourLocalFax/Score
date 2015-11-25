using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Middle
{
    using Front.Parse;
    using Front.Parse.SyntaxTree;

    using Symbols;

    internal sealed class KitAnalyzer : ModAnalyzer
    {
        public KitAnalyzer(DetailLogger log, SymbolTable symbols)
            : base(log, symbols)
        {
        }
    }
}
