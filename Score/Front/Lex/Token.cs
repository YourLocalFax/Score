using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Lex
{
    // TODO(kai): should I remove the sub-classes? They're pretty useless outside of
    // keeping fields out of the Token class.
    // I guess they're good for the ToString implementations, but that seems like it.

    internal class Token
    {
        public enum Type
        {
            UNKNOWN, // DUN DUN DUNNNNNN

            WILDCARD, // '_'

            #region Builtins
            PRIMITIVE,
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

        public readonly Type type;
        public readonly Span span;

        public virtual string Image => ToString();

        // TODO(kai): maybe just consolidate things, subclasses might not be nice.
        public Token(Type type, Span span)
        {
            this.type = type;
            this.span = span;
        }

        public override string ToString()
        {
            switch (type)
            {
                case Type.LPAREN: return "(";
                case Type.RPAREN: return ")";
                case Type.LBRACKET: return "[";
                case Type.RBRACKET: return "]";
                case Type.LBRACE: return "{";
                case Type.RBRACE: return "}";
                case Type.WILDCARD: return "_";
                default: return "<no idea, friend>";
            }
        }
    }

    internal sealed class TokenPrimitiveTyName : Token
    {
        private readonly string image;
        public override string Image => image;

        public TokenPrimitiveTyName(Span span, string image)
            : base(Type.PRIMITIVE, span)
        {
            this.image = image;
        }

        public override string ToString() => image;
    }

    /// <summary>
    /// Keywords are reserved and may never be used as identifiers.
    /// </summary>
    internal sealed class TokenKw : Token
    {
        private readonly string image;
        public override string Image => image;

        public TokenKw(Type type, Span span, string image)
            : base(type, span)
        {
            this.image = image;
        }

        public override string ToString() => image;
    }

    /// <summary>
    /// Identifiers are not reserved, but MAY hold special meaning in different context.
    /// </summary>
    internal class TokenId : Token
    {
        private readonly string image;
        public override string Image => image;

        public TokenId(Span span, string image)
            : base(Type.IDENT, span)
        {
            this.image = image;
        }

        public override string ToString() => image;
    }

    internal sealed class TokenSym : TokenId
    {
        public TokenSym(Span span, string image)
            : base(span, image) { }

        public override string ToString() => "'" + Image;
    }

    internal sealed class TokenStr : Token
    {
        public readonly string value;
        public readonly bool verbatim, cstr;

        public TokenStr(Span span, string value, bool verbatim, bool cstr)
            : base(Type.STR, span)
        {
            this.value = value;
            this.verbatim = verbatim;
            this.cstr = cstr;
        }

        public override string ToString() =>
            string.Format("{0}{1}\"{2}\"", verbatim ? "v" : "", cstr ? "c" : "", value);
    }

    internal sealed class TokenOp : Token
    {
        private readonly string image;
        public override string Image => image;

        public TokenOp(Span span, string image)
            : base(Type.OP, span)
        {
            this.image = image;
        }

        public TokenOp(Type type, Span span, string image)
            : base(type, span)
        {
            this.image = image;
        }

        public override string ToString() => image;
    }

    internal sealed class TokenChar : Token
    {
        public readonly uint value;

        public TokenChar(Span span, uint value)
            : base(Type.CHAR, span)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return string.Format("'{0}'", char.ConvertFromUtf32((int)value));
        }
    }

    internal sealed class TokenInt : Token
    {
        public readonly ulong n;
        public readonly string image;
        public readonly string suffix;

        public TokenInt(Span span, ulong n, string image, string suffix)
            : base(Type.INT, span)
        {
            this.n = n;
            this.image = image;
            this.suffix = suffix;
        }

        public override string ToString()
        {
            return image + suffix;
        }
    }

    internal sealed class TokenFloat : Token
    {
        public readonly double n;
        public readonly string image;
        public readonly string suffix;

        public TokenFloat(Span span, double n, string image, string suffix)
            : base(Type.FLOAT, span)
        {
            this.n = n;
            this.image = image;
            this.suffix = suffix;
        }

        public override string ToString()
        {
            return image + suffix;
        }
    }
}
