using Lex;
using Source;
using Ty;

namespace Ast
{
    using Data;

    public sealed class NodeFnDecl : Node
    {
        public MemberHeader header;
        public Spanned<Token> @fn;
        public NameOrOp name;
        public FnTyRef ty;
        public FnBody body;

        public string Name => name.Image;

        public ParameterList Parameters => ty.parameters;
        public Parameter ReturnParameter => ty.returnParameter;

        public override Span Span
        {
            get
            {
                // TODO(kai): Handle the member header span
                return new Span(@fn.span.fileName, @fn.span.start, body.Span.end);
            }
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
