namespace Score.Front.Parse.Ty
{
    using Data;

    internal sealed class TyOrVoid
    {
        public static TyOrVoid FromTy(QualifiedNameWithTyArgs name) => new TyOrVoid(name);
        public static TyOrVoid FromVoid(Span span) => new TyOrVoid(span);

        private readonly QualifiedNameWithTyArgs nameWithTyArgs;
        public readonly Span voidSpan;

        public bool IsTy => nameWithTyArgs != null;
        public bool IsVoid => nameWithTyArgs == null;

        public QualifiedName Name => nameWithTyArgs.name;
        public TyArgList TyArgs => nameWithTyArgs.tyArgs;

        public bool IsBuiltinTy => Name.Count == 1 && Name[0].id.IsBuiltin;

        private TyOrVoid(QualifiedNameWithTyArgs name)
        {
            this.nameWithTyArgs = name;
        }

        private TyOrVoid(Span voidSpan)
        {
            this.voidSpan = voidSpan;
        }

        public override string ToString()
        {
            return IsVoid ? "()" : nameWithTyArgs.ToString();
        }
    }
}
