using CodeAnalysis.Syntax.Expressions;

namespace CodeAnalysis.Syntax;

public interface ISyntaxExpressionVisitor<out TResult>
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