﻿namespace SyntaxTree
{
    public interface IAstVisitor
    {
        void Visit(Ast node);

        void Visit(NodeFnDecl fn);
        void Visit(NodeTypeDef typeDef);
        void Visit(NodeLet let);

        void Visit(NodeId id);
        void Visit(NodeBool b);
        void Visit(NodeInt i);
        void Visit(NodeStr s);
        void Visit(NodeEnclosed enc);
        void Visit(NodeTuple tuple);

        void Visit(NodeIndex index);
        void Visit(NodeInvoke invoke);
        void Visit(NodeInfix infix);
        void Visit(NodeSuffix suffix);

        void Visit(NodeRet ret);
        void Visit(NodeIf @if);
    }
}
