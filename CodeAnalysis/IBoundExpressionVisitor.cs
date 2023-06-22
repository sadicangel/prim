using CodeAnalysis.Binding;

namespace CodeAnalysis;

internal interface IBoundExpressionVisitor<out TResult>
{
    TResult Visit(BoundNeverExpression expression);
    TResult Visit(BoundUnaryExpression expression);
    TResult Visit(BoundBinaryExpression expression);
    TResult Visit(BoundLiteralExpression expression);
    TResult Visit(BoundVariableExpression expression);
    TResult Visit(BoundAssignmentExpression expression);
    TResult Visit(BoundIfExpression expression);
    TResult Visit(BoundCallExpression expression);
    TResult Visit(BoundConvertExpression expression);
}