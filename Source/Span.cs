namespace Source
{
    public struct Location
    {
        public uint line, column;

        public Location(uint line, uint column)
        {
            this.line = line;
            this.column = column;
        }

        public override string ToString() =>
            string.Format("line {0}, column {1}", line, column);
    }

    public struct Span
    {
        public string fileName;
        public Location start, end;

        public Span(string fileName, Location start, Location end)
        {
            this.fileName = fileName;
            this.start = start;
            this.end = end;
        }

        public override string ToString() =>
            string.Format("in {0}: {1} to {2}", fileName, start.ToString(), end.ToString());
    }
}
