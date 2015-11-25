using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front
{
    /// <summary>
    /// A span represents the region between two points in a source file.
    /// </summary>
    internal struct Span
    {
        public static bool operator ==(Span a, Span b)
        {
            return a.start == b.start && a.end == b.end;
        }

        public static bool operator !=(Span a, Span b)
        {
            return a.start != b.start || a.end != b.end;
        }

        private Location start;
        public Location Start
        {
            get { return start; }
            set
            {
                if (value > end)
                    throw new ArgumentException("Start must be less than or equal to end of span.");
                start = value;
            }
        }

        private Location end;
        public Location End
        {
            get { return end; }
            set
            {
                if (start > value)
                    throw new ArgumentException("Start must be less than or equal to end of span.");
                end = value;
            }
        }

        public Span(Location start, Location end)
        {
            if (start > end)
                throw new ArgumentException("Start must be less than or equal to end of span.");
            this.start = start;
            this.end = end;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            return this == (Span)obj;
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(start, end);
        }

        public override string ToString()
        {
            // TODO(kai): make use of the end Location.
            return string.Format("in file {0}, at line {1} column {2}",
                System.IO.Path.GetFileName(start.File), start.Line, start.Column);
        }

        public void Set(Location start, Location end)
        {
            if (start > end)
                throw new ArgumentException("Start must be less than or equal to end of span.");
            this.start = start;
            this.end = end;
        }
    }
}
