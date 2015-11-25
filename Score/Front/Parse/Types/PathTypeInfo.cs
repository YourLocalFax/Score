namespace Score.Front.Parse.Types
{
    internal class PathTypeInfo : TypeInfo
    {

        public readonly IdentPath path;

        public PathTypeInfo(IdentPath path)
        {
            this.path = path;
        }

        public override string ToString()
        {
            return path.ToString();
        }
    }
}
