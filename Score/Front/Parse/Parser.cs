using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Score.Front.Parse
{
    using Data;
    using Lex;
    using SyntaxTree;
    using Ty;

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
        private void Backup() => tokens.Backup();

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

        private bool CheckBuiltin() => HasCurrent && Current is TokenPrimitiveTyName;

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
            var result = (Current as TokenId).Image;
            Advance();
            return result;
        }

        private string GetOp()
        {
            string result;
            if (Check(PIPE))
                result = "|";
            else result = (Current as TokenOp).Image;
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
                    return ParseFn(mods);
                case TYPE:
                    return ParseTypeDef(mods);
                case LET:
                    // TODO(kai): make sure there are NO mods.
                    return ParseLet();
                default:
                    return ParseExpr();
            }
        }

        private Modifiers ParseMods()
        {
            var mods = new Modifiers();
            // pub priv extern intern
            while (HasCurrent)
            {
                switch (Current.type)
                {
                    case PUB: case PRIV: case EXTERN: case INTERN:
                        mods.modifiers.Add(Current as TokenKw);
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

        private QualifiedName ParseQualifiedName()
        {
            var result = new QualifiedName();

            // TODO(kai): clean this up some, there's gotta be a nicer looking way.
            while (Check(IDENT) || CheckBuiltin() || CheckOp())
            {
                // Add the name to the path
                if (Check(IDENT))
                    result.Add(new Name(Current as TokenId));
                else if (CheckBuiltin())
                    result.Add(new Name(Current as TokenPrimitiveTyName));
                else result.Add(new Op(Current as TokenOp));
                // Move past it
                Advance();
                // If there's a dot, we expect the path to continue some how.
                if (Check(DOT))
                {
                    // Skip it
                    Advance();
                    // Make sure the path continues, else we're fak'd
                    if (!(Check(IDENT) || CheckOp()))
                    {
                        log.Error(Current.span, "Expected identifier, symbol, or operator to coninue path.");
                        break;
                    }
                }
                // we should be done! happy days!
                else break;
            }

            return result;
        }

        private QualifiedNameWithTyArgs ParseQualifiedNameWithTyArgs()
        {
            var result = new QualifiedNameWithTyArgs();
            result.name = ParseQualifiedName();
            return result;
        }

        private Parameter ParseParameter()
        {
            bool hasName = false;
            bool hasTy = true;

            if (Check(IDENT))
            {
                Advance();
                hasName = CheckOp(COLON) || CheckOp(COMMA);
                hasTy = CheckOp(COLON) || !CheckOp(COMMA);
                Backup();
            }

            Name name = null;
            if (hasName)
            {
                name = new Name(Current as TokenId);
                Advance();
                if (hasTy)
                    AdvanceOp(COLON);
            }

            Spanned<TyRef> ty = null;
            if (hasTy)
                ty = ParseTy(); // TODO(kai): make this actually take a spanned plz

            return new Parameter(name, ty);
        }

        /// <summary>
        /// This expects the arguments to be surrounded by pipes.
        /// </summary>
        private ParameterList ParseParameterList()
        {
            var result = new ParameterList();

            // first pipe
            if (HasCurrent)
                result.leadingPipe = Current as TokenOp;
            ExpectOp(PIPE, "Expected '|' to start parameter list.");

            if (!CheckOp(PIPE))
            {
                while (HasCurrent)
                {
                    result.Add(ParseParameter());
                    if (CheckOp(COMMA))
                        AdvanceOp(COMMA);
                    else break;
                }
            }

            // last pipe
            if (HasCurrent)
                result.trailingPipe = Current as TokenOp;
            ExpectOp(PIPE, "Expected '|' to end parameter list.");
            return result;
        }

        private Spanned<TyRef> ParseTy()
        {
            var start = Current.span.Start;
            if (Check(LPAREN) && NextCheck(RPAREN))
            {
                var span = start + Next.span.End;
                Advance();
                Advance();
                return new Spanned<TyRef>(start + GetLastSpan().End, TyRef.VoidTy);
            }

            if (!HasCurrent)
            {
                log.Error(GetSpan(), "Expected type, found end of source.");
                return null;
            }

            // TODO(kai): if (CheckOp(CARET) || CheckOp(AMP))
            if (CheckOp(CARET))
            {
                var isPointer = CheckOp(CARET);
                AdvanceOp(isPointer ? CARET : AMP);
                bool isMut = false;
                if (Check(MUT))
                {
                    Advance();
                    isMut = true;
                }
                var type = ParseTy();
                // TODO(kai): return isPointer ? TyRef.PointerTo(type, isMut) as TyRef : TyRef.ReferenceTo(type, isMut) as TyRef;
                return new Spanned<TyRef>(start + type.span.End, TyRef.PointerTo(type.value, isMut) as TyRef);
            }

            switch (Current.type)
            {
                /*
                case LBRACKET:
                {
                    Advance();
                    var type = ParseTy();
                    var depth = 1u;
                    while (CheckOp(COMMA))
                    {
                        AdvanceOp(COMMA);
                        depth++;
                    }
                    Expect(RBRACKET, "Expected ']' to end array type definition.");
                    return TyRef.ArrayOf(type, depth);
                }
                case LPAREN:
                {
                    Advance();
                    var types = new List<TyRef>();
                    while (HasCurrent)
                    {
                        var type = ParseTy();
                        types.Add(type);
                        if (CheckOp(COMMA))
                            AdvanceOp(COMMA);
                        else break;
                    }
                    Expect(RPAREN, "Expected ')' to end tuple type definition.");
                    return TyRef.TupleOf(types.ToArray());
                }
                */
                case IDENT:
                    {
                        var span = Current.span;
                        var name = Current.Image;
                        Advance();
                        return new Spanned<TyRef>(span, new PathTyRef(span, name));
                    }
                case PRIMITIVE:
                {
                    var tok = Current as TokenPrimitiveTyName;
                    Advance();
                    return new Spanned<TyRef>(GetLastSpan(), TyRef.For(TyVariant.GetForPrimitive(tok)));
                }
                default:
                {
                    log.Error(GetSpan(), "Failed to parse type.");
                    return null;
                    //var name = ParseQualifiedNameWithTyArgs();
                    //return new Spanned<TyRef>(name.Span, TyRef.For(TyVariant.GetFor(name)));
                }
            }
        }

        /// <summary>
        /// This starts at the 'fn' keyword and reads until the return type.
        /// </summary>
        private void ParseFnDecl(NodeFnDecl fn)
        {
            // TODO(kai): check for no tokens?

            // The 'fn' keyword
            fn.fn = Current as TokenKw;
            Expect(FN, "Expected 'fn' when parsing fn declaration.");

            // Next, there'll be generic type arguments.
            // BUT WE DON'T CARE YET
            // TODO(kai): generic type arguments.

            // After that we expect a name:
            if (!HasCurrent)
            {
                log.Error(GetLastSpan(), "Expected an identifier to name the function, got end of file.");
                return;
            }

            fn.nameWithTyArgs = ParseQualifiedNameWithTyArgs();

            // Then, the parameter list!
            var parameters = ParseParameterList();
            Token arrow = null;
            Parameter returnTy = null;

            // Next, the return type!
            if (CheckOp(ARROW))
            {
                arrow = Current as Token;
                AdvanceOp(ARROW);
                returnTy = ParseParameter();
            }
            else returnTy = null;

            fn.ty = new TyFn(parameters, returnTy);

            // That's the end of the fn decl!
        }

        /// <summary>
        /// This can return either a NodeFN or a NodeFnDecl, depending on the precense of a body.
        /// This can still return a NodeFn for extern fns, it'll just end up being an error later (unless I do
        /// some kinda neat thing with bodied externs?)
        /// </summary>
        private NodeFnDecl ParseFn(Modifiers mods)
        {
            var fn = new NodeFnDecl();
            fn.header = new MemberHeader(mods);
            ParseFnDecl(fn);

            if (!HasCurrent)
                return fn;

            // TODO(kai): clean this up, it's ugly
            if (Check(EQ) || Check(LBRACE))
                fn.body = ParseFnBody();

            // TODO(kai): check if this is followed by an expression.
            // if it is, then we should parse that and use it as a body!

            return fn;
        }

        private FnBody ParseFnBody()
        {
            var body = new FnBody();

            /*
                fn.body = new FunctionBody();
                fn.body.eq = Current;
                Advance();
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
            */

            var hasEq = false;
            if (Check(EQ))
            {
                hasEq = true;
                body.eq = Current;
                Advance();
            }
            
            if (hasEq && !Check(LBRACE))
            {
                body.Add(ParseExpr());
                return body;
            }

            // TODO(kai): Maybe I need a break. check this out?
            if (HasCurrent)
                body.lbrace = Current;
            Expect(LBRACE, !hasEq ? "Expected '{' to start function body when '=' is not present." :
                "Expected '{' to start function body.");

            while (HasCurrent && !Check(RBRACE))
            {
                var node = ParseTopLevel();
                if (node == null)
                    break;
                body.Add(node);
                if (Check(RBRACE))
                    break;
            }

            if (HasCurrent)
                body.rbrace = Current;
            Expect(RBRACE, "Expected '}' to end function body block.");

            return body;
        }

        private NodeTypeDef ParseTypeDef(Modifiers mods)
        {
            var type = new NodeTypeDef();
            type.mods = mods;
            type.type = Current as TokenKw;
            Advance();

            type.name = new Name(ExpectIdent("Identifier expected to name the type."));

            if (HasCurrent)
                type.eq = Current;
            Expect(EQ, "Expected '='.");

            type.spTy = ParseTy();

            return type;
        }

        private NodeLet ParseLet()
        {
            Advance();

            // TODO(kai): this is temp, but be careful
            var name = new Name(ExpectIdent("Expected ident for let binding name."));
            Spanned<TyRef> ty = null;
            if (CheckOp(COLON))
            {
                Advance();
                ty = ParseTy();
            }

            var binding = new Parameter(name, ty);

            Expect(EQ, "Expected '=' in let binding.");

            var value = ParseExpr();

            return new NodeLet(binding, value);
        }
        #endregion
    }
}
