using System;

namespace Score.Front.Parse.SyntaxTree
{
    using Data;
    using Lex;
    using Ty;

    internal sealed class NodeFnDecl : Node
    {
        public MemberHeader header;
        public TokenKw @fn;
        public QualifiedNameWithTyArgs nameWithTyArgs;
        public ParameterList parameters;
        public Token arrow;
        // NOTE(kai): If this is null, we infer the type
        public TyRef returnTy;
        public FnBody body;

        public bool InferReturnTy => returnTy == null;

        public string Name => nameWithTyArgs.name[0].id.Image;

        // TODO(kai): figure out span
        internal override Span Span
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
