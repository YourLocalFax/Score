namespace Score
{
    using Front.Lex;

    using static Front.Lex.Token.Type;

    internal static class Util
    {
        public static string TokenTypeToString(Token.Type type, string @default = null)
        {
            switch (type)
            {
                case EQ: return "=";
                case DOT: return ".";
                case COMMA: return ",";
                case COLON: return ":";
                case PIPE: return "|";
                case AMP: return "&";
                case CARET: return "^";
                case ARROW: return "->";
                default: return @default;
            }
        }
    }
}
