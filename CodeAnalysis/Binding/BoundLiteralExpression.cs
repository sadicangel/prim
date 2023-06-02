namespace CodeAnalysis.Binding;

internal sealed record class BoundLiteralExpression(object? Value) : BoundExpression(BoundNodeKind.LiteralExpression, Value?.GetType() ?? typeof(object))
{
    public override T Accept<T>(IBoundExpressionVisitor<T> visitor) => visitor.Visit(this);
}
