using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Middle.Symbols
{
    using Back.LLVM.Values;
    using Back.Types;
    using Front.Parse;

    internal sealed class Symbol
    {
        public enum Kind
        {
            KIT,
            MOD,
            VAR,
            FN,
            // TODO(kai): other data types that need it
        }

        public readonly string name;
        public readonly Kind kind;
        public readonly ScoreType type;
        public readonly Mods mods;

        public Symbol(string name, Kind kind, ScoreType type, Mods mods)
        {
            this.name = name;
            this.kind = kind;
            this.type = type;
            this.mods = mods;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
