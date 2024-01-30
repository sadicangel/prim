namespace CodeAnalysis.Syntax.Expressions;
public abstract record class Expression(
    SyntaxNodeKind NodeKind,
    SyntaxTree SyntaxTree)
:
    SyntaxNode(NodeKind, SyntaxTree);
