using CodeAnalysis.Syntax;

namespace CodeAnalysis;

public interface IExpressionVisitor<out TResult>
{
    TResult Visit(Expression expression);

    TResult Visit(GroupExpression expression);

    TResult Visit(UnaryExpression expression);

    TResult Visit(BinaryExpression expression);

    TResult Visit(LiteralExpression expression);
}