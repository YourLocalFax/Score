namespace Score.Front.Parse.Data
{
    using Ty;

    internal sealed class Parameter
    {
        public readonly TyRef ty;
        public readonly Name name;

        // TODO(kai): default values

        public Parameter(TyRef ty, Name name)
        {
            this.ty = ty;
            this.name = name;
        }
    }
}
