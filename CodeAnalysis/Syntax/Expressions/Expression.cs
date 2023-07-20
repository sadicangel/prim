namespace CodeAnalysis.Syntax.Expressions;

public abstract record class Expression(SyntaxNodeKind NodeKind, SyntaxTree SyntaxTree) : SyntaxNode(NodeKind, SyntaxTree)
{
    public abstract T Accept<T>(ISyntaxExpressionVisitor<T> visitor);
}