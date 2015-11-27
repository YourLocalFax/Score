namespace Score.Front.Parse.Data
{
    using Ty;

    internal sealed class Parameter
    {
        public readonly TyRef ty;
        public readonly Name id;

        // TODO(kai): default values

        public Parameter(TyRef ty, Name id)
        {
            this.ty = ty;
            this.id = id;
        }
    }
}
