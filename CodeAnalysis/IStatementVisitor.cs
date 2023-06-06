using CodeAnalysis.Syntax;

namespace CodeAnalysis;
public interface IStatementVisitor<out TResult>
{
    TResult Accept(BlockStatement statement);
    TResult Accept(DeclarationStatement statement);
    TResult Accept(ExpressionStatement statement);
    TResult Accept(IfStatement statement);
    TResult Accept(WhileStatement statement);
    TResult Accept(ForStatement statement);
}
