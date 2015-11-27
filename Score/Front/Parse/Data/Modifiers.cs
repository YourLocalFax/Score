using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse.Data
{
    using Lex;

    internal sealed class Modifiers
    {
        public readonly List<TokenKw> mods = new List<TokenKw>();

        public Modifiers()
        {
        }

        public bool Has(Token.Type modifierType)
        {
            foreach (var mod in mods)
                if (mod.type == modifierType)
                    return true;
            return false;
        }
    }
}
