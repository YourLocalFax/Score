using System.Collections.Generic;
using System.Linq;

using SyntaxTree;
using SyntaxTree.Data;
using Lex;
using Log;
using Source;
using Ty;

using static Lex.TokenType;

namespace Parse
{
    public sealed class Parser
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
            return new Span(fileName, new Location(1, 1), new Location(1, 1));
        }

        private void AdvanceOp(TokenType opType) => AdvanceOp(Lex.Util.TokenTypeToString(opType));
        private void AdvanceOp(string image)
        {
            var curImage = Current.ToString();
            if (curImage.Length == image.Length)
                Advance();
            else
            {
                var start = Current.span.start;
                tokens.SetCurrent(LexerUtil.GetOpToken(new Span(fileName,
                    new Location(start.line, (uint)(start.column + image.Length)), Current.span.end),
                    curImage.Substring(image.Length)));
            }
        }

        private bool Check(TokenType type) => HasCurrent && Current.type == type;

        private bool NextCheck(TokenType type) => HasNext && Next.type == type;

        private bool CheckIdent() => Check(IDENT);

        private bool CheckBuiltin() => HasCurrent && Current.type == BUILTIN_TY_NAME;

        private bool CheckOp() => HasCurrent && Current.IsOp;

        private bool CheckOp(TokenType opType) => CheckOp(Lex.Util.TokenTypeToString(opType));
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
            var result = Current.Image;
            Advance();
            return result;
        }

        private string GetOp()
        {
            string result = Current.Image;
            Advance();
            return result;
        }

        private bool Expect(TokenType type, string message)
        {
            if (!Check(type))
            {
                log.Error(GetSpan(), message);
                return false;
            }
            Advance();
            return true;
        }

        private bool Expect(TokenType type, string format, params object[] args) =>
            Expect(type, string.Format(format, args));

        private Token ExpectIdent(string format, params object[] args)
        {
            var c = Current;
            if (!Expect(IDENT, format, args))
                return null;
            return c;
        }

        private bool ExpectOp(TokenType opType, string message) => ExpectOp(Lex.Util.TokenTypeToString(opType), message);
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
            while (HasCurrent)
            {
                switch (Current.type)
                {
                    case PUB: case PRIV: case EXTERN: case INTERN:
                        Spanned<string> optArg = null;
                        if (Check(STR))
                        {
                            optArg = Current.Image.Spanned(Current.span);
                            Advance();
                        }
                        mods.Add(Current, optArg);
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

        private List<Node> ParseBlock()
        {
            Expect(LBRACE, "Expected '{' to start block.");

            List<Node> block = new List<Node>();
            while (HasCurrent && !Check(RBRACE))
            {
                var node = ParseTopLevel();
                if (node == null)
                    break;
                block.Add(node);
                if (Check(RBRACE))
                    break;
            }

            Expect(RBRACE, "Expected '}' to end block.");
            return block;
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
                case IF:
                {
                    var @if = new NodeIf();
                    while (Check(IF))
                    {
                        Advance();
                        @if.conditions.Add(new NodeIf.IfBlock(ParseExpr(), ParseBlock()));
                        if (Check(EL))
                        {
                            if (!Check(IF))
                            {
                                @if.fail = ParseBlock();
                                break;
                            }
                            else Advance();
                        }
                    }
                    result = @if;
                } break;
                case LPAREN:
                    var lparen = Current;
                    Advance();
                    if (Check(RPAREN))
                    {
                        result = new NodeTuple(lparen, Current, new List<NodeExpr>());
                        Advance();
                        break;
                    }
                    bool trailingComma;
                    var exprs = ParseCommaList(out trailingComma);
                    if (exprs.Count == 1 && !trailingComma)
                    {
                        Expect(RPAREN, "Expected ')' to match opening '('.");
                        result = new NodeEnclosed(lparen, Last, exprs[0]);
                    }
                    else
                    {
                        Expect(RPAREN, "Expected ')' to close tuple.");
                        result = new NodeTuple(lparen, Last, exprs);
                    }
                    break;
                case TRUE: case FALSE:
                {
                    result = new NodeBool(Current);
                    Advance();
                } break;
                case INT:
                {
                    result = new NodeInt(Current);
                    Advance();
                } break;
                case STR:
                {
                    result = new NodeStr(Current);
                    Advance();
                } break;
                case IDENT:
                {
                    result = new NodeId(Current);
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
                var dot = Current;
                Advance();
                result = new NodeIndex(result, dot, new NodeId(ExpectIdent("Identifier expected for type index.")));
            }

            return ParseInvoke(result, isEnclosed);
        }

        private NodeExpr ParseInvoke(NodeExpr node, bool isEnclosed)
        {
            if (!HasCurrent)
                return node;
            // TODO(kai): this is ugly, please fix it. Also submit feature request?
            NodeExpr expr;
            if ((Current.span.start.line == node.Span.start.line || isEnclosed) &&
                (expr = ParsePrimaryExpr(isEnclosed, false)) != null)
            {
                List<NodeExpr> args = new List<NodeExpr>();
                args.Add(expr);
                while ((Current.span.start.line == node.Span.start.line || isEnclosed) &&
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
            if (HasCurrent && CheckOp() && Current.span.start.line == left.Span.start.line)
            {
                var op = Current;
                Advance();
                if (HasCurrent)
                {
                    bool isOnSameLine = Current.span.start.line == op.span.start.line;
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

        private Parameter ParseParameter()
        {
            bool hasName = false;
            bool hasTy = true;

            if (Check(IDENT))
            {
                Advance();
                hasName = CheckOp(COLON) || Check(COMMA);
                hasTy = CheckOp(COLON) || !Check(COMMA);
                Backup();
            }

            Spanned<string> name = null;
            if (hasName)
            {
                name = Current.Image.Spanned(Current.span);
                Advance();
                if (hasTy)
                    AdvanceOp(COLON);
            }

            Spanned<TyRef> ty;
            if (hasTy)
                ty = ParseTy();
            else ty = (InferTyRef.InferTy as TyRef).Spanned(default(Span));

            return new Parameter(name, ty);
        }

        /// <summary>
        /// This expects the arguments to be surrounded by pipes.
        /// </summary>
        private ParameterList ParseParameterList()
        {
            var result = new ParameterList();

            // first pipe
            ExpectOp(PIPE, "Expected '|' to start parameter list.");

            if (!CheckOp(PIPE))
            {
                while (HasCurrent)
                {
                    result.Add(ParseParameter());
                    if (Check(COMMA))
                        Advance();
                    else break;
                }
            }

            // last pipe
            ExpectOp(PIPE, "Expected '|' to end parameter list.");
            return result;
        }

        private Spanned<TyRef> ParseTy()
        {
            var start = Current.span.start;
            if (Check(LPAREN) && NextCheck(RPAREN))
            {
                var span = new Span(fileName, start, Next.span.end);
                Advance();
                Advance();
                return new Spanned<TyRef>(new Span(fileName, start, GetLastSpan().end), TyVoid.VoidTy);
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
                return new Spanned<TyRef>(new Span(fileName, start, type.span.end), new PointerTyRef(type.value, isMut));
            }

            switch (Current.type)
            {
                case IDENT:
                {
                    var span = Current.span;
                    var name = Current.Image;
                    Advance();
                    return new Spanned<TyRef>(span, new PathTyRef(name));
                }
                case BUILTIN_TY_NAME:
                {
                    var name = Current.Image;
                    Advance();
                    return new Spanned<TyRef>(GetLastSpan(), BuiltinTyRef.GetByName(name));
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
            fn.fn = Current;
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

            // TODO(kai): support operators here, too.
            fn.name = NameOrOp.FromName(ExpectIdent("Expected an identifier to name the function").Image);

            // Then, the parameter list!
            fn.parameters = ParseParameterList();
            var paramTys = fn.parameters.Select(param => param.Ty).ToList();

            TyRef returnTy = InferTyRef.InferTy;

            // Next, the return type!
            if (CheckOp(ARROW))
            {
                AdvanceOp(ARROW);
                fn.@return = ParseParameter();
                returnTy = fn.@return.Ty;
            }
            else returnTy = null;

            fn.ty = new FnTyRef(paramTys, returnTy);

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
            type.modifiers = mods;
            type.type = Current;
            Advance();

            var name = ExpectIdent("Identifier expected to name the type.");
            type.name = name.Image.Spanned(name.span);

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
            var name = ExpectIdent("Expected ident for let binding name.");
            Spanned<TyRef> ty;
            if (CheckOp(COLON))
            {
                Advance();
                ty = ParseTy();
            }
            else ty = (InferTyRef.InferTy as TyRef).Spanned(default(Span));

            var binding = new Parameter(name.Image.Spanned(name.span), ty);

            Expect(EQ, "Expected '=' in let binding.");

            var value = ParseExpr();

            return new NodeLet(binding, value);
        }
        #endregion
    }
}
