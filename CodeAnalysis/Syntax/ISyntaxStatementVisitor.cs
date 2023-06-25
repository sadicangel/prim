namespace CodeAnalysis.Syntax;
public interface ISyntaxStatementVisitor<out TResult>
{
    TResult Visit(BlockStatement statement);
    TResult Visit(DeclarationStatement statement);
    TResult Visit(ExpressionStatement statement);
    TResult Visit(IfStatement statement);
    TResult Visit(WhileStatement statement);
    TResult Visit(ForStatement statement);
}
