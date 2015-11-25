using System.Collections.Generic;

namespace Score.Front.Parse.Patterns
{
    internal sealed class TuplePattern : BasePattern
    {
        public List<BasePattern> patterns;

        public TuplePattern(List<BasePattern> patterns)
        {
            this.patterns = patterns;
        }
    }
}
