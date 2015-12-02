namespace Score.Front.Parse.SyntaxTree
{
    using Lex;

    internal sealed class NodeBool : NodeExpr
    {
        public TokenKw token;

        public bool Value => token.Image == "true";
        public override Span Span => token.span;

        public NodeBool(TokenKw token)
        {
            this.token = token;
        }

        public override void Accept(IAstVisitor visitor) => visitor.Visit(this);
    }
}
