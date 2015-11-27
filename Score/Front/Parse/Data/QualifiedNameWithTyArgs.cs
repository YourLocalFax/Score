namespace Score.Front.Parse.Data
{
    internal sealed class QualifiedNameWithTyArgs
    {
        public QualifiedName name;
        public TyArgList tyArgs;

        public override string ToString()
        {
            // TODO(kai): add tyArgs to ToString
            return name.ToString();
        }
    }
}
