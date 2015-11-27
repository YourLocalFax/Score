namespace Score.Front.Parse.Ty
{
    using Data;
    using Lex;

    internal sealed class TyOrVoid
    {
        public static TyOrVoid FromTy(QualifiedNameWithTyArgs name) => new TyOrVoid(name);
        public static TyOrVoid FromVoid(Span span) => new TyOrVoid(span);

        public readonly QualifiedNameWithTyArgs name;
        public readonly Span voidSpan;

        public bool IsTy => name != null;
        public bool IsVoid => name == null;

        public bool IsBuiltinTy => name.name.names.Count == 1 && name.name.names[0].id.IsBuiltin;

        private TyOrVoid(QualifiedNameWithTyArgs name)
        {
            this.name = name;
        }

        private TyOrVoid(Span voidSpan)
        {
            this.voidSpan = voidSpan;
        }
    }
}
