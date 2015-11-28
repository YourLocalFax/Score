namespace Score.Middle.Symbols
{
    using Front.Parse.Data;
    using Front.Parse.Ty;

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
        public readonly TyRef ty;
        public readonly Modifiers mods;

        public Symbol(string name, Kind kind, TyRef ty, Modifiers mods)
        {
            this.name = name;
            this.kind = kind;
            this.ty = ty;
            this.mods = mods;
        }

        public override string ToString() => name;
    }
}
