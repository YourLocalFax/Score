﻿namespace Score.Front.Parse
{
    using SyntaxTree;

    interface IAstVisitor
    {
        void Visit(Ast node);

        void Visit(NodeFnDecl fnDecl);
        void Visit(NodeFn fn);

        void Visit(NodeId id);
        void Visit(NodeInt i);
        void Visit(NodeStr s);
        void Visit(NodeTuple tuple);

        void Visit(NodeIndex index);
        void Visit(NodeInvoke invoke);
        void Visit(NodeInfix infix);
        void Visit(NodeSuffix suffix);
    }
}