namespace Ast.Data
{
    public sealed class MemberHeader
    {
        // TODO(kai): annotations plz
        public readonly Modifiers modifiers;

        public MemberHeader(Modifiers modifiers)
        {
            this.modifiers = modifiers;
        }
    }
}
