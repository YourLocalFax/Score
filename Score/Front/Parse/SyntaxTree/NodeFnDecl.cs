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
        public TyFn ty;
        public FnBody body;

        public string Name => nameWithTyArgs.name[0].id.Image;

        public ParameterList Parameters => ty.parameters;
        public Parameter ReturnParameter => ty.returnParameter;

        // TODO(kai): figure out span
        internal override Span Span
        {
            get
            {
                return @fn.span; // TODO(kai): ACTUALLY DO SPAN PLS
            }
        }

        public override void Accept(IAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
