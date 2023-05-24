namespace CodeAnalysis.Syntax;

public abstract record class Expression(NodeKind Type) : Node(Type)
{
    public abstract T Accept<T>(IExpressionVisitor<T> visitor);
}
