namespace CodeAnalysis.Syntax;

public abstract record class Expression(NodeKind Kind) : Node(Kind)
{
    public abstract T Accept<T>(IExpressionVisitor<T> visitor);
}