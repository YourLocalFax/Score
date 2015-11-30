namespace Score.Front.Parse.Data
{
    using Ty;

    internal sealed class Parameter
    {
        public readonly Name name;
        public TyRef ty;

        public bool IsNamed => name != null;

        // TODO(kai): default values

        public Parameter(Name name, TyRef ty)
        {
            this.ty = ty;
            this.name = name;
        }
    }
}
