﻿using Lex;
using Source;
using Ty;

namespace SyntaxTree
{
    using Data;

    public sealed class NodeTypeDef : Node
    {
        public Modifiers mods;
        public Token type;
        public Spanned<string> name;
        public Token eq;
        public Spanned<TyRef> spTy;

        public TyRef Ty => spTy.value;

        public override Span Span => new Span(name.span.fileName, name.span.start, spTy.span.end);

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
