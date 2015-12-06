using System;

using Source;

namespace Lex
{
    using static TokenType;

    // TODO(kai): Probably clean up the TokenType enum a bit.

    public enum TokenType
    {
        UNKNOWN, // DUN DUN DUNNNNNN

        WILDCARD, // '_'

        #region Builtins
        BUILTIN_TY_NAME,
        #endregion

        #region Keywords
        LET,
        MUT,
        LAZY,
        TAKE,
        PUB,
        PRIV,
        STAT,
        EXTERN,
        INTERN,
        VIRTUAL,
        OVERIDE,
        IMPLICIT,

        USE,
        FROM,
        KIT,

        SELF,
        TAILREC,
        FN,
        GEN,
        NEW,
        VOID,

        THIS,
        BASE,
        STRUCT,
        CLASS,
        DATA,
        ENUM,
        TRAIT,
        IMPL,
        TYPE,
        SEALED,
        PARTIAL,

        IF,
        EL,
        WHEN,
        FOR,
        EACH,
        WHILE,
        LOOP,
        MATCH,
        RET,
        CONT,
        BREAK,
        RESUME,
        YIELD,
        #endregion

        IDENT,

        EQ, // '='
        DOT, // '.'
        COMMA, // ','
        COLON, // ':'
        PIPE, // '|'
        AMP, // '&'
        CARET, // '^'
        ARROW, // '->'
               // TODO(kai): other reserved operator chars

        OP,

        LPAREN, // '('
        RPAREN, // ')'
        LBRACKET, // '['
        RBRACKET, // ']'
        LBRACE, // '{'
        RBRACE, // '}'

        STR,
        TRUE,
        FALSE,
        INT,
        FLOAT,
        CHAR,
    }

    public sealed class Token
    {
        #region Factory Methods
        /// <summary>
        /// Create a new token.
        /// This is basically an alias for the private constructor.
        /// The only resong for the existence of this method is consistency
        /// with the other means of constructing a token.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        internal static Token New(TokenType type, Span span, string image) =>
            new Token(type, span, image ?? "<no idea, friend>");

        /// <summary>
        /// Returns a new token for a builtin type name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static Token NewBuiltinTyName(Span span, string name) =>
            new Token(BUILTIN_TY_NAME, span, name);

        /// <summary>
        /// Returns a new token for a keyword.
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        internal static Token NewKeyword(TokenType keyword, Span span, string image) =>
            new Token(keyword, span, image);

        /// <summary>
        /// Returns a new token for an identifier.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        internal static Token NewIdentifier(Span span, string image) =>
            new Token(IDENT, span, image);

        /// <summary>
        /// Returns a new token for a symbol.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        internal static Token NewSymbol(Span span, string image) =>
            new Token(IDENT, span, image, tok => "'" + tok.Image);

        /// <summary>
        /// Returns a new token for a string literal.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="verbatim"></param>
        /// <param name="cstr"></param>
        /// <returns></returns>
        internal static Token NewString(Span span, string value, bool verbatim, bool cstr)
        {
            var res = new Token(STR, span, value, tok =>
                string.Format("{0}{1}\"{2}\"", tok.StrVerbatim ? "v" : "", tok.StrC ? "c" : "", value));
            res.StrVerbatim = verbatim;
            res.StrC = cstr;
            return res;
        }

        /// <summary>
        /// Returns a new token for an operator.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        internal static Token NewOperator(Span span, string image)
        {
            var result = new Token(OP, span, image);
            result.IsOp = true;
            return result;
        }

        /// <summary>
        /// Returns a new token for a specific reserved operator.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        internal static Token NewOperator(TokenType type, Span span, string image)
        {
            var result = new Token(type, span, image);
            result.IsOp = true;
            return result;
        }

        /// <summary>
        /// Returns a new token for an identifier operator.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        internal static Token NewIdentifierOperator(Span span, string image)
        {
            var result = new Token(OP, span, image, tok => "`" + tok.Image);
            result.IsOp = true;
            return result;
        }

        /// <summary>
        /// Returns a new token for a character literal.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static Token NewChar(Span span, uint value)
        {
            var res = new Token(CHAR, span, char.ConvertFromUtf32((int)value), tok =>
                string.Format("'{0}'", tok.Image));
            res.CharValue = value;
            return res;
        }

        /// <summary>
        /// Returns a new token for an integer literal.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="image"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        internal static Token NewInteger(Span span, ulong value, string image, string suffix)
        {
            var res = new Token(INT, span, image, tok => tok.Image + tok.NumericSuffix);
            res.IntegerValue = value;
            res.NumericSuffix = suffix;
            return res;
        }

        /// <summary>
        /// Returns a new token for a float literal.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="image"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        internal static Token NewFloat(Span span, double value, string image, string suffix)
        {
            var res = new Token(FLOAT, span, image, tok => tok.Image + tok.NumericSuffix);
            res.FloatValue = value;
            res.NumericSuffix = suffix;
            return res;
        }
        #endregion

        #region Fields
        /// <summary>
        /// The type of this token.
        /// </summary>
        public readonly TokenType type;

        /// <summary>
        /// Where in the source file this token is.
        /// </summary>
        public readonly Span span;

        private readonly Func<Token, string> dbg;
        #endregion

        #region Properties
        /// <summary>
        /// Special string data representing this token.
        /// Note that this is not a representation as it appears in source code,
        /// this is designed to be useful to the parser.
        /// For a more accurate look at how the value may have appeared in source,
        /// use the ToString() method.
        /// </summary>
        public string Image { get; private set; }

        /// <summary>
        /// If this token represents a string literal,
        /// this denotes if the string is verbatim.
        /// </summary>
        public bool StrVerbatim { get; internal set; }
        /// <summary>
        /// If this token represents a string literal,
        /// this denotes if the string is a c-string.
        /// </summary>
        public bool StrC { get; internal set; }

        /// <summary>
        /// If this token represents a character literal,
        /// this is the numeric value it represents.
        /// </summary>
        public uint CharValue { get; internal set; }
        /// <summary>
        /// If this token represents a character literal,
        /// this denotes if the character is a c-character.
        /// </summary>
        public bool CharC { get; internal set; }

        /// <summary>
        /// If this token represents an integer literal,
        /// this is the absolute representation of that literal.
        /// </summary>
        public ulong IntegerValue { get; internal set; }
        /// <summary>
        /// If this token represents a float literal,
        /// this is the absolute representation of that literal.
        /// </summary>
        public double FloatValue { get; internal set; }
        /// <summary>
        /// If this token represents any numeric literal,
        /// this is the suffix, if any, attached to it.
        /// </summary>
        public string NumericSuffix { get; internal set; }

        /// <summary>
        /// If this token represents an operator,
        /// this is set to true.
        /// </summary>
        public bool IsOp { get; internal set; }
        #endregion

        #region Constructors
        private Token(TokenType type, Span span, string image, Func<Token, string> dbg = null)
        {
            this.type = type;
            this.span = span;
            Image = image;
            this.dbg = dbg ?? (tok => tok.Image);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns a more debug friendly representation of this token.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => dbg(this);
        #endregion
    }
}
