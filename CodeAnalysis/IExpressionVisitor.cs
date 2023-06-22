using CodeAnalysis.Syntax;

namespace CodeAnalysis;

public interface IExpressionVisitor<out TResult>
{
    TResult Visit(GroupExpression expression);

    TResult Visit(UnaryExpression expression);

    TResult Visit(BinaryExpression expression);

    TResult Visit(LiteralExpression expression);

    TResult Visit(AssignmentExpression expression);

    TResult Visit(NameExpression expression);
    TResult Visit(IfExpression expression);
    TResult Visit(CallExpression expression);
}