using System.Globalization;

namespace Lex
{
    using static TokenType;

    public static class Util
    {
        /// <summary>
        /// Returns true for unicode characters of classes Lu, Ll, Lt, Lm, Lo, or Nl.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsLetterChar(int c)
        {
            var cat = CharUnicodeInfo.GetUnicodeCategory(char.ConvertFromUtf32(c), 0);
            switch (cat)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.LetterNumber:
                    return true;
                default: return false;
            }
        }

        /// <summary>
        /// Returns true for unicode characters of classes Mn or Mc.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsCombiningChar(int c)
        {
            var cat = CharUnicodeInfo.GetUnicodeCategory(char.ConvertFromUtf32(c), 0);
            switch (cat)
            {
                case UnicodeCategory.SpacingCombiningMark:
                case UnicodeCategory.NonSpacingMark:
                    return true;
                default: return false;
            }
        }

        /// <summary>
        /// Returns true for unicode characters of class Nd.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsDecimalDigitChar(int c) =>
            CharUnicodeInfo.GetUnicodeCategory(char.ConvertFromUtf32(c), 0) == UnicodeCategory.DecimalDigitNumber;

        /// <summary>
        /// Returns true for unicode characters of class Pc.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsConnectingChar(int c) =>
            CharUnicodeInfo.GetUnicodeCategory(char.ConvertFromUtf32(c), 0) == UnicodeCategory.ConnectorPunctuation;

        /// <summary>
        /// Returns true for unicode characters of class Cf.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsFormattingChar(int c) =>
            CharUnicodeInfo.GetUnicodeCategory(char.ConvertFromUtf32(c), 0) == UnicodeCategory.Format;

        /// <summary>
        /// Returns true if the unicode character is a letter char or an underscore (U+005F).
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsIdentifierStart(int c) =>
            IsLetterChar(c) || c == '_';

        /// <summary>
        /// Returns true if the unicode character is an identifier start or
        /// a combining, decimal digit, connecting, or formatting char.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsIdentifierPart(int c) =>
            IsIdentifierStart(c) || IsCombiningChar(c) || IsDecimalDigitChar(c) || IsConnectingChar(c) || IsFormattingChar(c);

        /// <summary>
        /// Returns true if the character is a valid decimal digit for numeric literals.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsDigit(char c)
        {
            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns true if the character is a valid digit for numeric literals in the given radix.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsDigitInRadix(char c, int radix)
        {
            switch (radix)
            {
                case 16:
                    switch (c)
                    {
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                        case 'F':
                            return true;
                        default: return IsDecimalDigitChar(c);
                    }
                case 8: return IsDigit(c) && c < '8';
                case 2: return IsDigit(c) && c < '2';
            }
            return false;
        }

        /// <summary>
        /// Returns true if the character is valid for a number prefix, false otherwise.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsNumPrefix(char c)
        {
            return GetNumPrefixRadix(c) != 0;
        }

        /// <summary>
        /// Returns the integer radix that a given character represents for numeric literals.
        /// Returns 0 if the character is not a valid number prefix.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int GetNumPrefixRadix(char c)
        {
            switch (c)
            {
                case 'x': case 'X': return 16;
                case 'c': case 'C': return 8;
                case 'b': case 'B': return 2;
                default: return 0;
            }
        }

        /// <summary>
        /// Returns true if the character is a valid operator character.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsOperator(char c)
        {
            // Check every valid operator character
            // Some of these will end up being reserved, but all are viable for custom operators.
            switch (c)
            {
                case '`':
                case '~':
                case '!':
                case '@':
                case '#':
                case '$':
                case '%':
                case '^':
                case '&':
                case '*':
                case '-':
                case '=':
                case '+':
                case '\\':
                case '|':
                case ';':
                case ':':
                case '<':
                case '.':
                case '>':
                case '/':
                case '?':
                    return true;
                default: return false;
            }
        }

        /// <summary>
        /// Returns true if the image represents a primitive type name.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsBuiltinTyName(string s)
        {
            switch (s)
            {
                case "i8":
                case "u8":
                case "i16":
                case "u16":
                case "i32":
                case "u32":
                case "i64":
                case "u64":
                case "f16":
                case "f32":
                case "f64":
                case "bool": return true;
                default: return false;
            }
        }

        /// <summary>
        /// Returns true if the given image is a keyword.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsKw(string s) =>
            GetTypeFromKeyword(s) != UNKNOWN;

        /// <summary>
        /// Returns the token type associated with the given keyword
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static TokenType GetTypeFromKeyword(string s)
        {
            switch (s)
            {
                /*
                let|mut|lazy|take|pub|priv|stat|extern|virtual|override|implicit // these last three are idunno
                use|from|kit
                self|tailrec|fn|gen|new|void
                this|base|struct|class|data|enum|trait|impl|type|sealed|partial
                if|el|when|for|each|while|loop|match|ret|break|cont|resume|yield
                typeof|is|as
                */

                case "true": return TRUE;
                case "false": return FALSE;

                case "let": return LET;
                case "mut": return MUT;
                case "lazy": return LAZY;
                case "take": return TAKE;
                case "pub": return PUB;
                case "priv": return PRIV;
                case "stat": return STAT;
                case "extern": return EXTERN;
                case "intern": return INTERN;
                case "virtual": return VIRTUAL;
                case "override": return OVERIDE;
                case "implicit": return IMPLICIT;

                case "use": return USE;
                case "from": return FROM;
                case "kit": return KIT;

                case "self": return SELF;
                case "tailrec": return TAILREC;
                case "fn": return FN;
                case "gen": return GEN;
                case "new": return NEW;
                case "void": return VOID;

                case "this": return THIS;
                case "base": return BASE;
                case "struct": return STRUCT;
                case "class": return CLASS;
                case "data": return DATA;
                case "enum": return ENUM;
                case "trait": return TRAIT;
                case "impl": return IMPL;
                case "type": return TYPE;
                case "sealed": return SEALED;
                case "partial": return PARTIAL;

                case "if": return IF;
                case "el": return EL;
                case "when": return WHEN;
                case "for": return FOR;
                case "each": return EACH;
                case "while": return WHILE;
                case "loop": return LOOP;
                case "match": return MATCH;
                case "ret": return RET;
                case "break": return BREAK;
                case "cont": return CONT;
                case "resume": return RESUME;
                case "yield": return YIELD;

                default: return UNKNOWN;
            }
        }

        public static string TokenTypeToString(TokenType type, string @default = null)
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
