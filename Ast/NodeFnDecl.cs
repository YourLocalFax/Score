using System.Collections.Generic;

using Lex;
using Source;
using Ty;

namespace SyntaxTree
{
    using Data;

    public sealed class NodeFnDecl : Node
    {
        public MemberHeader header;
        public Token @fn;
        public NameOrOp name;
        public List<string> parameterNames;
        public string returnName;
        public FnTyRef ty;
        public FnBody body;

        public string Name => name.Image;

        public List<TyRef> ParameterTys => ty.parameterTys;
        public TyRef ReturnTy => ty.returnTy;

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
