using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Log;
using Source;

namespace Lex
{
    using static TokenType;

    using static Util;
    using static LexerUtil;

    internal static class LexerUtil
    {
        public static Token GetOpToken(Span span, string image)
        {
            // TODO(kai): make sure we have all special operators
            switch (image)
            {
                case "=":  return Token.NewOperator(EQ, span, image);
                case ":":  return Token.NewOperator(COLON, span, image);
                case ".":  return Token.NewOperator(DOT, span, image);
                case "|":  return Token.NewOperator(PIPE, span, image);
                case "&":  return Token.NewOperator(AMP, span, image);
                case "^":  return Token.NewOperator(CARET, span, image);
                case "->": return Token.NewOperator(ARROW, span, image);
                default:   return Token.NewOperator(span, image);
            }
        }
    }

    public sealed class Lexer
    {
        /// <summary>
        /// Returns a list of all tokens in the given file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public TokenList GetTokens(DetailLogger log, string fileName, Encoding encoding = null)
        {
            // TODO(kai): check that a file exists.
            // also, just more error checking is a good idea.

            var result = new List<Token>();
            // Use a separate state so that this object itself can be used in multiple threads, hopefully.
            var state = new LexerState(log, fileName, encoding);

            while (true)
            {
                var token = state.GetToken();
                // If no token was returned, just break.
                // If errors happened, they should be addressed outside the lexer.
                if (token == null)
                    break;
                result.Add(token);
                //Console.WriteLine(token);
            }

            return new TokenList(result);
        }
    }

    internal sealed class LexerState
    {
        private readonly DetailLogger log;
        private readonly string fileName;

        private int sourceOffset; // TODO(kai): or something
        private readonly string source;

        private uint line = 1, col = 0;
        private char c = '\0';

        private StringBuilder builder = new StringBuilder();

        private bool EndOfSource { get; set; } = false;

        public LexerState(DetailLogger log, string fileName, Encoding encoding)
        {
            this.log = log;
            source = File.ReadAllText(this.fileName = fileName, encoding ?? Encoding.UTF8);
            Advance();
        }

        private Location GetLocation() =>
            new Location(line, col);

        private Span GetSpan() =>
            new Span(fileName, GetLocation(), GetLocation());

        // TODO(kai): rename this, GetString is very vague.
        private string GetString()
        {
            var res = builder.ToString();
            builder.Clear();
            return res;
        }

        private void Advance()
        {
            // TODO(kai): cache the length in a field? shouldn't be too bad but we can check later.
            if (sourceOffset >= source.Length)
            {
                EndOfSource = true;
                return;
            }
            c = source[sourceOffset++];
            if (c == '\n')
            {
                line++;
                col = 0;
            }
            else col++;
        }

        private char Peek()
        {
            return Peek(1);
        }

        private char Peek(int offset)
        {
            if (offset <= 0)
                throw new ArgumentException("offset must be positive.");
            if (EndOfSource || sourceOffset + offset >= source.Length)
                return '\0';
            return source[sourceOffset + offset - 1];
        }

        // TODO(kai): Should this return a bool? It might be nice for conditional lexing...
        private void ExpectAndAdvance(char e)
        {
            if (c != e)
            {
                log.Error(GetSpan(), "Unexpected character '{0}', expected '{1}'.", c, e);
                // return false;
            }
            Advance(); // e
            // return true;
        }

        /// <summary>
        /// Appends the current character to the string builder and advances to the next character.
        /// </summary>
        private void Bump()
        {
            builder.Append(c);
            Advance();
        }

        /// <summary>
        /// Appends the given character and does not advance to the next character.
        /// </summary>
        /// <param name="c"></param>
        private void Bump(char c)
        {
            builder.Append(c);
        }

        /// <summary>
        /// Appends the given character as utf-32 and does not advance to the next character.
        /// </summary>
        /// <param name="c"></param>
        private void Bump(uint c)
        {
            builder.Append(char.ConvertFromUtf32((int)c));
        }

        private bool IsNotIdentStart()
        {
            // TODO(kai): I don't think this should be a switch/case.
            // The if statement earlier was good.
            switch (c)
            {
                case 'v':
                    switch (Peek())
                    {
                        case 'c': return Peek(2) == '"';
                        case '"': return true;
                        default: return false;
                    }
                case 'c':
                    switch (Peek())
                    {
                        case 'v': return Peek(2) == '"';
                        case '"': return true;
                        default: return false;
                    }
                default: return false;
            }
        }

        private bool IsCommentStart()
        {
            if (c == '/')
                return Peek() == '#' || Peek() == '*';
            return false;
        }

        private bool IsOpIdentStart()
        {
            if (c == '`')
            {
                if (IsIdentifierStart(Peek()))
                {
                    if (Peek() == '_')
                        return IsIdentifierPart(Peek(2));
                    return true;
                }
            }
            return false;
        }

        // TODO(kai): this should really just return a token, Result is getting annoying in C#
        public Token GetToken()
        {
            if (EndOfSource)
                // Returning error false means it was intended.
                return null;

            // where this token, no matter what, should start (hopefully, dunno yet actually).
            var start = GetLocation();

            // This should be an identifier or keyword unless
            // it's v", in which case it should be a verbatim string
            // (handled below)
            if (IsIdentifierStart(c) && !IsNotIdentStart())
            {
                var str = LexIdentStr();
                var span = new Span(fileName, start, GetLocation());
                // _ is a special token called Wildcard, much like how * works in other environments.
                if (str == "_")
                    return Token.New(WILDCARD, span, "_");
                // otherwise, it's an identifier
                if (IsKw(str))
                    return Token.NewKeyword(GetTypeFromKeyword(str), span, str);
                else if (IsBuiltinTyName(str))
                    return Token.NewBuiltinTyName(span, str);
                return Token.NewIdentifier(span, str);
            }

            // Should be a number literal, so get that.
            if (IsDigit(c))
                return LexNumLiteral();

            if (IsOperator(c) && !IsCommentStart() && !IsOpIdentStart())
                return LexOperator();

            switch (c)
            {
                // Let's handle whitespace, it should be completely ignored.
                // TODO(kai): this can probably go in a separate method EatWhitespace, but meh for now.
                case ' ': case '\t': case '\r': case '\n':
                    // skip the whitespace
                    Advance();
                    // Attempt to return a token, please.
                    return GetToken();
                // Other things
                case '`':
                    return LexIdentOperator();
                case '/':
                    if (Peek() == '#')
                    {
                        // Lex past the line comment, then continue.
                        EatLineComment();
                        return GetToken();
                    }
                    else if (Peek() == '*')
                    {
                        EatBlockComment();
                        return GetToken();
                    }
                    // If it's not a comment, it'll go into the operator lexer.
                    // We don't have one yet, so TODO(kai): operator lexing.
                    // just return null until then.
                    return null;
                // These should NOT be identifiers, those are checked above.
                // These should turn into cstr/verbatim strings.
                case 'c':
                    {
                        Advance(); // 'c'
                        bool verbatim = c == 'v';
                        if (verbatim)
                            Advance(); // 'v'
                        var str = LexStrLiteral(verbatim);
                        var span = new Span(fileName, start, GetLocation());
                        return Token.NewString(span, str, verbatim, true);
                    }
                case 'v':
                    {
                        Advance(); // 'v'
                        bool cstr = c == 'c';
                        if (cstr)
                            Advance(); // 'c'
                        var str = LexStrLiteral(true);
                        var span = new Span(fileName, start, GetLocation());
                        return Token.NewString(span, str, true, cstr);
                    }
                // Just a normal string literal
                case '"':
                    {
                        var str = LexStrLiteral(false);
                        var span = new Span(fileName, start, GetLocation());
                        return Token.NewString(span, str, false, false);
                    }
                case '\'': // TODO(kai): Not sure what kind of modifiers we can have on chars.
                    return LexCharLiteralOrSymbol();
                case ',':
                    Advance();
                    return Token.New(COMMA, new Span(fileName, start, GetLocation()), ",");
                case '(':
                    Advance();
                    return Token.New(LPAREN, new Span(fileName, start, GetLocation()), "(");
                case ')':
                    Advance();
                    return Token.New(RPAREN, new Span(fileName, start, GetLocation()), ")");
                case '[':
                    Advance();
                    return Token.New(LBRACKET, new Span(fileName, start, GetLocation()), "[");
                case ']':
                    Advance();
                    return Token.New(RBRACKET, new Span(fileName, start, GetLocation()), "]");
                case '{':
                    Advance();
                    return Token.New(LBRACE, new Span(fileName, start, GetLocation()), "{");
                case '}':
                    Advance();
                    return Token.New(RBRACE, new Span(fileName, start, GetLocation()), "}");
                default:
                    // TODO(kai): fatal error, we don't know how to continue.
                    return null;
            }
        }

        private void EatLineComment()
        {
            while (!EndOfSource && c != '\n')
                Advance();
            if (!EndOfSource)
                Advance(); // here, this should be a \n
        }

        private void EatBlockComment()
        {
            // This method considers the actual block comment part of the nesting,
            // that makes it really clean to lex.
            var nesting = 0;
            while (!EndOfSource)
            {
                var c = this.c;
                Advance();
                if (c == '/' && this.c == '*')
                {
                    Advance(); // '*'
                    nesting++;
                }
                if (c == '*' && this.c == '/')
                {
                    Advance(); // '/'
                    if (--nesting == 0)
                        return;
                }
            }
            // TODO(kai): error here, unfinished block comment
        }

        private string LexIdentStr()
        {
            if (!IsIdentifierStart(c))
            {
                log.Error(GetSpan(), "Invalid start to an identifier.");
                return "";
            }
            var start = GetLocation();
            while (!EndOfSource && IsIdentifierPart(c))
                Bump();
            return GetString();
        }

        private Token LexNumLiteral()
        {
            // TODO(kai): _ should be allowed as a separator in numbers, do that eventually.
            // ALSO UPDATE LexPrefixedInt WHEN YOU DO

            // Next up, gotta make sure that the radix stuff is ALWAYS integer, and allow different chars for 0x

            // TODO(kai): prefixes like 0x, 0c, 0b need to be checked.
            // The radix can simply be stored as an int and we can effectively ignore the prefix after that.

            var start = GetLocation();

            int radix = 10;
            string image = "";

            // TODO(kai): I don't actually like how this looks...
            // For now, it should work.
            if (c == '0')
            {
                // I need to check that this is in the form 0 radix-char digit first
                if (IsNumPrefix(Peek()) && IsDigitInRadix(Peek(2), 16))
                {
                    // skips 0
                    Advance();
                    radix = GetNumPrefixRadix(c);
                    // skips the radix character
                    Advance();
                    // It's a prefix we can use, not a suffix.
                    // for example, 0x should be 0 with the suffix "x".
                    // Of course, 0xF is NOT 0 with the suffix "xF"
                    image = LexPrefixedInt(radix);
                }
                // otherwise, we fall through and continue as normal.
            }

            // For now I think this'll be the easiest way to check for things like 1.0.0, which is not a valid number literal.
            // This means things like 1.0.abs() or something would be valid. (rare cases for method calls mostly, occasionally fields if I add any to numbers)
            bool isFloat = false;

            // Only do this if the radix IS 10.
            // This is probably not a great way to handle this, sure
            // but it'll work for now.
            // Refactoring can be done later
            // TODO(kai): probably refactor this.
            if (radix == 10)
            {

                // I wonder if this can actually lex integers yet, actually.
                while (!EndOfSource)
                {
                    // Digits should be allowed here, we can check special cases later.
                    if (IsDigit(c))
                    {
                        Bump();
                        // can't forget this, whoops.
                        continue;
                    }
                    // Doing this, I can check all cases and hopefully that covers everything.
                    switch (c)
                    {
                        // one posibility for floats
                        case '.':
                            // I think we should also check if this is followed by a character. 1.field should be valid, not a literal with errors
                            if (isFloat || IsIdentifierStart(Peek()))
                                // exit our loop plz
                                goto loop_end;
                            isFloat = true;
                            Bump();
                            break;
                        // another posibility for floats
                        case 'e':
                        case 'E':
                            // TODO After marking as a float, I should check the exponent
                            isFloat = true;
                            Bump();
                            // signs are allowed in exponents, so check those here and only here.
                            if (c == '+' || c == '-')
                                Bump();
                            break;
                        default:
                            // DUH
                            goto loop_end;
                    }
                }
                loop_end:
                image = GetString();
            }

            // at this point, image should always contain a valid value.

            // The span of this literal
            var end = GetLocation();

            // HERE we get the suffix
            var suffix = LexOptionalNumSuffix();
            if (isFloat)
            {
                // Here, we need to parse our float.
                // I'm using ulong because the LLVM wrapper will expect all floats to be double eventually, and I don't
                // need to use them before then.
                double value = Convert.ToDouble(image);
                // NOTE end is saved for use elsewhere, mostly. Might be removed, use GetLocation()
                var span = new Span(fileName, start, GetLocation());
                return Token.NewFloat(span, value, image, suffix);
            }
            else
            {
                // Here, we need to parse our integer.
                // I'm using ulong because the LLVM wrapper will expect all integers to be ulong eventually, and I don't
                // need to use them before then.
                // NOTE that this should never fail to parse.
                // we JUST lexed the number, and it should fit C# just fine.
                // If that happens to NOT be the case in the future, a different parse method should be added
                // to handle Score specific numbers.
                ulong value = Convert.ToUInt64(image, radix);
                // NOTE end is saved for use elsewhere, mostly. Might be removed, use GetLocation()
                var span = new Span(fileName, start, GetLocation());
                return Token.NewInteger(span, value, image, suffix);
            }
        }

        private string LexPrefixedInt(int radix)
        {
            // The prefix is already handled for us
            while (!EndOfSource && (IsDigit(c) || IsDigitInRadix(c, radix)))
            {
                if (!IsDigitInRadix(c, radix))
                {
                    log.Error(GetSpan(), "The digit {0} is not valid in the current radix (you are using radix {1}).",
                        c, radix);
                    // 0b123 is one token, but it's an invalid token
                    return null;
                }
                Bump();
            }
            return GetString();
        }

        /// <summary>
        /// Optionally lexes a suffix (basically, just an identifier) after a number.
        /// This is used to determine the type of the literal.
        /// The built in numeric types are the only types that expect suffixes by default.
        /// Those types are ((u|i)(8|16|32|64))|f(16|32|64)).
        /// </summary>
        /// <returns></returns>
        private string LexOptionalNumSuffix()
        {
            // can't possibly be a suffix.
            if (!IsIdentifierStart(c))
                return "";

            // OH, that makes it easy actually.
            // If I allow any suffix to be "valid", then number literals can be allowed in the lang.
            // things like 102030val where val is a typename would allow for literal construction or something.
            // I think I actually like that.

            return LexIdentStr();
        }

        private Token LexOperator()
        {
            var start = GetLocation();

            // Get the operator for as long as there are operator characters
            while (!EndOfSource && IsOperator(c))
                Bump();
            // What the operator looks like now
            var image = GetString();
            var span = new Span(fileName, start, GetLocation());
            return GetOpToken(span, image);
        }

        private Token LexIdentOperator()
        {
            var start = GetLocation();
            Advance(); // '`'
            var image = LexIdentStr();
            var span = new Span(fileName, start, GetLocation());
            return Token.NewIdentifierOperator(span, image);
        }

        private string LexStrLiteral(bool verbatim)
        {
            // I also want to support c-string literals, so maybe that should be a parameter here, too.

            // Basically a verbatim string allows newlines and \'s
            // A c-string would compile down to the representation of a c-string, an array of u8's,
            // rather than a Score string instance.

            Advance(); // '"'

            while (!EndOfSource)
            {
                // TODO(kai): should this be in a switch?

                // "He said ""Hello, world!"" and then left."
                if (c == '"' && Peek() != '"')
                    break;
                if (c == '\\')
                {
                    bool fail;
                    // TODO(kai): look into refactoring this, it's hideous :c
                    if (verbatim)
                    {
                        if (Peek() == '\\')
                        {
                            Advance();
                            var value = LexEscapeSequence(out fail);
                            if (!fail)
                                Bump();
                        }
                        else Bump();
                    }
                    else
                    {
                        var value = LexEscapeSequence(out fail);
                        if (!fail)
                            Bump();
                    }
                }
                if (c == '\n' && !verbatim)
                    break;
                Bump();
            }

            Advance(); // '"'

            return GetString();
        }

        private Token LexCharLiteralOrSymbol()
        {
            var start = GetLocation();
            Advance(); // '''
            // It CAN'T be a symbol, so attempt to lex a char literal
            if (!IsIdentifierStart(c) || Peek() == '\'')
            {
                bool fail;
                var c = LexCharLiteral(out fail);
                var span = new Span(fileName, start, GetLocation());
                return Token.NewChar(span, c);
            }
            else
            {
                var sym = LexIdentStr();
                var span = new Span(fileName, start, GetLocation());
                return Token.NewSymbol(span, sym);
            }
        }

        private uint LexCharLiteral(out bool fail)
        {
            fail = false;

            uint value;
            switch (c) // Any other casesthat should be checked? I know chars are simple but.
            {
                case '\\':
                    value = LexEscapeSequence(out fail);
                    break;
                default:
                    value = c;
                    Advance();
                    break;
            }

            ExpectAndAdvance('\'');
            Advance(); // '''
            return value;
        }

        private uint LexEscapeSequence(out bool fail)
        {
            fail = false;
            Advance(); // '\'
            switch (c)
            {
                case '0':
                    Advance();
                    return '\0';
                case 'r':
                    Advance();
                    return '\r';
                case 'n':
                    Advance();
                    return '\n';
                case 't':
                    Advance();
                    return '\t';
                case '\'':
                    Advance();
                    return '\'';
                case '"':
                    Advance();
                    return '\"';
                // Any other important escapes I should worry about now?
                case 'x':
                    // TODO(kai): hex codes plz. This will probably be unicode, so we don't need \u0000 for that.
                    // Until then, fall through with an error.
                default:
                    log.Error(GetSpan(), "Invalid escape sequence character '{0}'.", c);
                    Advance();
                    fail = true;
                    return '?'; // doesn't matter what we return.
            }
        }
    }
}
