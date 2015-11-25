namespace Score.Front.Parse.Types
{
    internal sealed class PointerType : TypeInfo
    {
        public TypeInfo type;
        public bool isMut;

        public PointerType(TypeInfo type, bool isMut)
        {
            this.type = type;
            this.isMut = isMut;
        }

        public override string ToString()
        {
            return string.Format("^{0}{1}", isMut ? "mut " : "", type.ToString());
        }
    }
}
