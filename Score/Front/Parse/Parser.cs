using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse
{
    using Lex;
    using Patterns;
    using SyntaxTree;
    using Types;

    using static Lex.Token.Type;

    internal sealed class Parser
    {
        public Ast Parse(DetailLogger log, TokenList tokens, string fileName)
        {
            var state = new ParserState(log, tokens, fileName);

            var ast = new Ast();
            // TODO(kai): actually run a parser loop.
            while (true)
            {
                //Console.WriteLine("Attempting to parse");
                var node = state.ParseTopLevel();
                if (node == null)
                    break;
                ast.children.Add(node);
            }

            return ast;
        }
    }

    internal sealed class ParserState
    {
        #region Fields
        private readonly DetailLogger log;
        private readonly TokenList tokens;
        private readonly string fileName;
        #endregion

        #region Properties
        private Token Last => tokens.Last;
        private Token Current => tokens.Current;
        private Token Next => tokens.Next;
        private bool HasLast => tokens.HasLast;
        private bool HasCurrent => tokens.HasCurrent;
        private bool HasNext => tokens.HasNext;
        #endregion

        public ParserState(DetailLogger log, TokenList tokens, string fileName)
        {
            this.log = log;
            this.tokens = tokens;
            this.fileName = fileName;
        }

        #region Delegate Methods
        private void Advance() => tokens.Advance();

        private Token Peek(int offset) => tokens.Peek(offset);
        #endregion

        #region Helper Methods
        private Span GetSpan()
        {
            if (HasCurrent)
                return Current.span;
            return GetLastSpan();
        }

        private Span GetLastSpan()
        {
            if (HasLast)
                return Last.span;
            return new Location(fileName, 1, 1).AsSpan();
        }

        private void AdvanceOp(Token.Type opType) => AdvanceOp(Util.TokenTypeToString(opType));
        private void AdvanceOp(string image)
        {
            var curImage = Current.ToString();
            if (curImage.Length == image.Length)
                Advance();
            else
            {
                var start = Current.span.Start;
                tokens.SetCurrent(LexerUtil.GetOpToken(curImage.Substring(image.Length),
                    new Location(start.File, start.Line, start.Column + image.Length) + Current.span.End));
            }
        }

        private bool Check(Token.Type type) => HasCurrent && Current.type == type;

        private bool NextCheck(Token.Type type) => HasNext && Next.type == type;

        private bool CheckIdent() => Check(IDENT);

        private bool CheckOp() => HasCurrent && Current is TokenOp;

        private bool CheckOp(Token.Type opType) => CheckOp(Util.TokenTypeToString(opType));
        private bool CheckOp(string image)
        {
            if (!CheckOp())
                return false;
            // FIXME(kai): all tokens should have "image"
            var curImage = Current.ToString();
            if (!curImage.StartsWith(image))
                return false;
            return true;
        }

        private string GetIdent()
        {
            var result = (Current as TokenId).image;
            Advance();
            return result;
        }

        private string GetOp()
        {
            string result;
            if (Check(PIPE))
                result = "|";
            else result = (Current as TokenOp).image;
            Advance();
            return result;
        }

        private bool Expect(Token.Type type, string message)
        {
            if (!Check(type))
            {
                log.Error(GetSpan(), message);
                return false;
            }
            Advance();
            return true;
        }

        private bool Expect(Token.Type type, string format, params object[] args) =>
            Expect(type, string.Format(format, args));

        private TokenId ExpectIdent(string format, params object[] args)
        {
            var c = Current;
            if (!Expect(IDENT, format, args))
                return null;
            return (c as TokenId);
        }

        private bool ExpectOp(Token.Type opType, string message) => ExpectOp(Util.TokenTypeToString(opType), message);
        private bool ExpectOp(string image, string message)
        {
            if (!CheckOp(image))
            {
                log.Error(GetSpan(), message);
                return false;
            }
            AdvanceOp(image);
            return true;
        }
        #endregion

        #region Parsing Methods
        /// <summary>
        /// This can return any top level node, but should only like statements.
        /// </summary>
        public Node ParseTopLevel()
        {
            //Console.WriteLine(Current);

            // No more tokens, don't error. Just return null and the loop knows to stop.
            if (!HasCurrent)
                return null;

            // TODO(kai): check modifiers, then those can propogate below.
            var mods = ParseMods();

            // TODO(kai): check for all top level statements and return them

            switch (Current.type)
            {
                case FN:
                    // NOTE eventually we'll have modifiers to worry about
                    return ParseFn(mods);
                case LET:
                    return ParseLet();
                default:
                    return ParseExpr();
            }
        }

        private Mods ParseMods()
        {
            var mods = new Mods();
            // pub priv extern intern
            while (HasCurrent)
            {
                switch (Current.type)
                {
                    case PUB: case PRIV: case EXTERN: case INTERN:
                        string error;
                        if (!mods.Set(Current.type, out error))
                            log.Error(GetSpan(), error);
                        Advance();
                        break;
                    default: return mods;
                }
            }
            return mods;
        }

        private NodeExpr ParseExpr(bool isEnclosed = false, bool doError = true)
        {
            var first = ParsePrimaryExpr(isEnclosed, doError);
            // TODO(kai): handle infix expressions
            if (first == null)
                return null;
            return ParseInfix(first, isEnclosed);
        }

        private List<NodeExpr> ParseCommaList(out bool trailingComma)
        {
            trailingComma = false;
            var result = new List<NodeExpr>();
            while (HasCurrent)
            {
                // TODO(kai): determine if enclosed
                var expr = ParseExpr(false, false);
                if (expr == null)
                    break;
                trailingComma = false;
                result.Add(expr);
                if (Check(COMMA))
                {
                    trailingComma = true;
                    Advance();
                }
                else break;
            }
            return result;
        }

        private NodeExpr ParsePrimaryExpr(bool isEnclosed, bool doError = true)
        {
            if (!HasCurrent)
            {
                log.Error(GetLastSpan(), "Expected a primary expression, got end of file.");
                return null;
            }

            NodeExpr result;
            switch (Current.type)
            {
                case LPAREN:
                    var start = Current.span.Start;
                    Advance();
                    if (Check(RPAREN))
                    {
                        // TODO(kai): return empty tuple.
                        result = new NodeTuple(new List<NodeExpr>(), start, Current.span.End);
                        Advance();
                    }
                    bool trailingComma;
                    var exprs = ParseCommaList(out trailingComma);
                    var end = Current.span.End;
                    if (exprs.Count == 1 && !trailingComma)
                    {
                        Expect(RPAREN, "Expected ')' to match opening '('.");
                        result = exprs[0];
                    }
                    else
                    {
                        Expect(RPAREN, "Expected ')' to close tuple.");
                        result = new NodeTuple(exprs, start, GetLastSpan().End);
                    }
                    break;
                case INT:
                {
                    result = new NodeInt(Current as TokenInt);
                    Advance();
                } break;
                case STR:
                {
                    result = new NodeStr(Current as TokenStr);
                    Advance();
                } break;
                case IDENT:
                {
                    result = new NodeId(Current as TokenId);
                    Advance();
                } break;
                default:
                    if (doError)
                    {
                        log.Error(GetLastSpan(), "Unexpected token '{0}' when parsing primary expression.", Current);
                        Advance();
                    }
                    return null;
            }

            // TODO(kai): check field index.
            while (Check(DOT))
            {
                Advance();
                result = new NodeIndex(result, new NodeId(ExpectIdent("Identifier expected for type index.")));
            }

            return ParseInvoke(result, isEnclosed);
        }

        private NodeExpr ParseInvoke(NodeExpr node, bool isEnclosed)
        {
            if (!HasCurrent)
                return node;
            // TODO(kai): this is ugly, please fix it. Also submit feature request?
            NodeExpr expr;
            if ((Current.span.Start.Line == node.Span.Start.Line || isEnclosed) &&
                (expr = ParsePrimaryExpr(isEnclosed, false)) != null)
            {
                List<NodeExpr> args = new List<NodeExpr>();
                args.Add(expr);
                while ((Current.span.Start.Line == node.Span.Start.Line || isEnclosed) &&
                    (expr = ParsePrimaryExpr(isEnclosed, false)) != null)
                {
                    args.Add(expr);
                }
                return new NodeInvoke(node, args);
            }
            return node;
        }

        private NodeExpr ParseInfix(NodeExpr left, bool isEnclosed)
        {
            if (HasCurrent && CheckOp() && Current.span.Start.Line == left.Span.Start.Line)
            {
                var op = Current as TokenOp;
                Advance();
                if (HasCurrent)
                {
                    bool isOnSameLine = Current.span.Start.Line == op.span.Start.Line;
                    if (isOnSameLine || isEnclosed)
                    {
                        var right = ParsePrimaryExpr(isEnclosed);
                        right = ParseInfix(right, isEnclosed);
                        return new NodeInfix(left, right, op);
                    }
                }
                return new NodeSuffix(left, op);
            }
            return left;
        }

        private IdentPath ParseIdentPath()
        {
            var result = new IdentPath();

            // TODO(kai): clean this up some, there's gotta be a nicer looking way.
            while (Check(IDENT) || Check(SYMBOL))
            {
                // Add the name to the path
                if (Check(IDENT))
                    result.Add((Current as TokenId).image);
                else result.Add((Current as TokenSym).image);
                // Move past it
                Advance();
                // If there's a dot, we expect the path to continue some how.
                if (Check(DOT))
                {
                    // Skip it
                    Advance();
                    // Make sure the path continues, else we're fak'd
                    // TODO(kai): maybe also accept operators? would be convenient
                    if (!(Check(IDENT) || Check(SYMBOL)))
                    {
                        log.Error(Current.span, "Expected identifier or symbol to coninue path");
                        break;
                    }
                }
                // we should be done! happy days!
                else break;
            }

            return result;
        }

        /// <summary>
        /// This expects the arguments to be surrounded by pipes.
        /// </summary>
        private List<ParamInfo> ParseParams()
        {
            // first pipe

            ExpectOp(PIPE, "Expected '|' to start parameter list.");

            var result = new List<ParamInfo>();
            if (!CheckOp(PIPE))
            {
                while (HasCurrent)
                {
                    var param = new ParamInfo();
                    result.Add(param);
                    // The name of this param
                    param.nameSpan = Current.span;
                    param.name = ExpectIdent("Identifier expected to name a parameter.").image;
                    if (CheckOp(COLON))
                    {
                        AdvanceOp(COLON);
                        // the type of this param
                        param.type = ParseType(out param.typeSpan);
                    }
                    if (CheckOp(COMMA))
                        AdvanceOp(COMMA);
                    else break;
                }
            }

            // last pipe
            ExpectOp(PIPE, "Expected '|' to end parameter list.");
            return result;
        }

        private TypeInfo ParseType(out Span span)
        {
            var start = Current.span.Start;
            if (Check(LPAREN) && NextCheck(RPAREN))
            {
                span = start + Next.span.End;
                Advance();
                Advance();
                return TypeInfo.VOID;
            }

            if (!HasCurrent)
            {
                // TODO(kai): error, no type to parse.
            }

            // FIXME(kai): lots of repeated code, fix this
            if (CheckOp(CARET))
            {
                AdvanceOp(CARET);
                bool isMut = false;
                if (Check(MUT))
                {
                    Advance();
                    isMut = true;
                }
                Span subSpan;
                var type = ParseType(out subSpan);
                span = start + subSpan.End;
                return new PointerType(type, isMut);
            }


            if (CheckOp(AMP))
            {
                AdvanceOp(AMP);
                bool isMut = false;
                if (Check(MUT))
                {
                    Advance();
                    isMut = true;
                }
                Span subSpan;
                var type = ParseType(out subSpan);
                span = start + subSpan.End;
                return new PointerType(type, isMut);
            }

            switch (Current.type)
            {
                case LBRACKET:
                {
                    Advance();
                    Span subSpan;
                    var type = ParseType(out subSpan);
                    Expect(RBRACKET, "Expected ']' to end array type definition.");
                    span = start + subSpan.End;
                    return new ArrayTypeInfo(type);
                }
                default:
                {
                    var path = ParseIdentPath();
                    span = start + GetSpan().End;
                    return new PathTypeInfo(path);
                }
            }
        }

        // TODO(kai): the actual parser stuff

        /// <summary>
        /// This starts at the 'fn' keyword and reads until the return type.
        /// </summary>
        private NodeFnDecl ParseFnDecl(Mods mods)
        {
            // TODO(kai): check for no tokens?
            var result = new NodeFnDecl(Current.span.Start, mods);

            // The 'fn' keyword
            Expect(FN, "Expected 'fn' when parsing fn declaration.");

            // Next, there'll be generic type arguments.
            // BUT WE DON'T CARE YET
            // TODO(kai): generic type arguments.

            // After that we expect a name:
            if (!HasCurrent)
            {
                log.Error(GetLastSpan(), "Expected an identifier to name the function, got end of file.");
                return result;
            }

            result.nameSpan = Current.span;
            if (CheckIdent())
                result.name = GetIdent();
            else if (CheckOp())
                result.name = GetOp();
            else
            {
                log.Error(Current.span, "Invalid function name '{0}'.", Current);
                Advance();
            }

            // Then, the parameter list!
            result.@params = ParseParams();

            // Next, the return type!
            if (CheckOp(ARROW))
            {
                AdvanceOp(ARROW);
                result.retType = ParseType(out result.retSpan);
            }
            else result.retType = TypeInfo.INFER;

            // That's the end of the fn decl!

            result.end = Last.span.End;
            return result;
        }

        /// <summary>
        /// This can return either a NodeFN or a NodeFnDecl, depending on the precense of a body.
        /// This can still return a NodeFn for extern fns, it'll just end up being an error later (unless I do
        /// some kinda neat thing with bodied externs?)
        /// </summary>
        private Node ParseFn(Mods mods)
        {
            var decl = ParseFnDecl(mods);

            if (!HasCurrent)
                return decl;

            // TODO(kai): clean this up, it's ugly
            if (Check(EQ))
            {
                var fn = new NodeFn(decl);
                Advance();
                fn.hasEq = true;
                if (Check(LBRACE))
                    fn.body = ParseFnBodyBlock();
                else
                {
                    fn.body = new List<Node>();
                    var expr = ParseExpr();
                    if (expr == null)
                    {
                        log.Error(HasCurrent ? Current.span : GetLastSpan(), "Expected an expression to complete function body.");
                        return fn;
                    }
                    fn.body.Add(expr);
                }
                fn.end = Last.span.End;
                return fn;
            }
            else if (Check(LBRACE))
            {
                var fn = new NodeFn(decl);
                fn.body = ParseFnBodyBlock();
                fn.end = Last.span.End;
                return fn;
            }

            // TODO(kai): check if this is followed by an expression.
            // if it is, then we should parse that and use it as a body!

            return decl;
        }

        private List<Node> ParseFnBodyBlock()
        {
            Advance(); // '{'
            var result = new List<Node>();

            while (HasCurrent && !Check(RBRACE))
            {
                var node = ParseTopLevel();
                if (node == null)
                    break;
                result.Add(node);
                if (Check(RBRACE))
                    break;
            }

            Expect(RBRACE, "Expected '}' to end function body block.");
            return result;
        }

        private NodeLet ParseLet()
        {
            Advance();

            ParsePattern();

            return null;
        }

        private BasePattern ParsePattern()
        {
            if (Check(LPAREN))
            {
                Advance();
                // TUPLE
                if (Check(RPAREN))
                {
                    // TODO(kai): error, empty pattern.
                }
                List<BasePattern> patterns = new List<BasePattern>();
                while (HasCurrent && !Check(RPAREN))
                {
                    var pattern = ParsePattern();
                    patterns.Add(pattern);
                    if (Check(COMMA))
                        Advance();
                    else break;
                }
                Expect(RPAREN, "Expected ')' to end tuple pattern.");
                return new TuplePattern(patterns);
            }
            Span span;
            var type = ParseType(out span);
            if (Check(LPAREN))
            {
                // IDENT(TUPLE)
                var tuple = ParsePattern() as TuplePattern;
                return new TypePattern(type, tuple);
            }
            if (type is PathTypeInfo)
            {
                var path = (type as PathTypeInfo).path;
                if (path.path.Count == 1)
                    return new Pattern(path.path[0]);
            }
            // TODO(kai): bad things.
            return null;
        }
        #endregion
    }
}
