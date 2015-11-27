namespace Score.Middle.Symbols
{
    using Back.Types;
    using Front.Parse.Data;

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
        public readonly Modifiers mods;

        public Symbol(string name, Kind kind, ScoreType type, Modifiers mods)
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
