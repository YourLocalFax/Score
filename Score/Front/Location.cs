using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front
{
    /// <summary>
    /// A location represents a single point in a source file.
    /// </summary>
    internal struct Location
    {
        public static Span operator +(Location a, Location b)
        {
            return new Span(a, b);
        }

        public static bool operator ==(Location a, Location b)
        {
            return a.File == b.File && a.line == b.line && a.column != b.column;
        }

        public static bool operator !=(Location a, Location b)
        {
            return a.File != b.File || a.line != b.line || a.column != b.column;
        }

        public static bool operator <(Location a, Location b)
        {
            return a.File == b.File && a.line <= b.line && a.column < b.column;
        }

        public static bool operator >(Location a, Location b)
        {
            return a.File == b.File && a.line >= b.line && a.column > b.column;
        }

        public static bool operator <=(Location a, Location b)
        {
            return a.File == b.File && a.line <= b.line && a.column <= b.column;
        }

        public static bool operator >=(Location a, Location b)
        {
            return a.File == b.File && a.line >= b.line && a.column >= b.column;
        }

        public string File { get; private set; }

        // YES, I could use a uint here. This is fine, though.

        private int line;
        public int Line
        {
            get { return line; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Line cannot be less than 1.");
                line = value;
            }
        }

        private int column;
        public int Column
        {
            get { return column; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Column cannot be negative.");
                column = value;
            }
        }

        public Location(string file, int line, int column)
        {
            // shut up, compiler.
            this.line = 0;
            this.column = 0;

            File = file;
            Line = line;
            Column = column;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return this == (Location)obj;
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(File, line, column);
        }

        public Span AsSpan()
        {
            return new Span(this, new Location(File, line, column + 1));
        }
    }
}
