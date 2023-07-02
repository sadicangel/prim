namespace CodeAnalysis.Syntax;
public interface ISyntaxStatementVisitor<out TResult>
{
    TResult Visit(FunctionDeclaration functionDeclaration);
    TResult Visit(VariableDeclaration statement);

    TResult Visit(BlockStatement statement);
    TResult Visit(ExpressionStatement statement);
    TResult Visit(IfStatement statement);
    TResult Visit(WhileStatement statement);
    TResult Visit(ForStatement statement);
    TResult Visit(BreakStatement statement);
    TResult Visit(ContinueStatement statement);
    TResult Visit(ReturnStatement statement);
}
