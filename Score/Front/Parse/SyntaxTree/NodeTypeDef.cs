﻿namespace Score.Front.Parse.SyntaxTree
{
    using Data;
    using Lex;
    using Ty;

    internal sealed class NodeTypeDef : Node
    {
        public Modifiers mods;
        public TokenKw type;
        public Name name;
        public Token eq;
        public Spanned<TyRef> ty;

        public TyRef Ty => ty.value;

        internal override Span Span => type.span.Start + ty.span.End;

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
