using CodeAnalysis.Syntax;

namespace CodeAnalysis;
public interface IStatementVisitor<out TResult>
{
    TResult Accept(BlockStatement statement);
    TResult Accept(ExpressionStatement statement);
}
