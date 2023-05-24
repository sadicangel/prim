using CodeAnalysis.Binding;

namespace CodeAnalysis;

public interface IBoundExpressionVisitor<out TResult>
{
    TResult Visit(BoundExpression expression);

    //TResult Visit(BoundGroupExpression expression);

    TResult Visit(BoundUnaryExpression expression);

    TResult Visit(BoundBinaryExpression expression);

    TResult Visit(BoundLiteralExpression expression);
}
