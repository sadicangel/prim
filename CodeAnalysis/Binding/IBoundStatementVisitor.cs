namespace CodeAnalysis.Binding;

internal interface IBoundStatementVisitor<out TResult>
{
    TResult Visit(BoundVariableDeclaration statement);
    TResult Visit(BoundFunctionDeclaration statement);
    TResult Visit(BoundBlockStatement statement);
    TResult Visit(BoundExpressionStatement statement);
    TResult Visit(BoundIfStatement statement);
    TResult Visit(BoundWhileStatement statement);
    TResult Visit(BoundForStatement statement);
    TResult Visit(BoundLabelStatement statement);
    TResult Visit(BoundGotoStatement statement);
    TResult Visit(BoundConditionalGotoStatement statement);
    TResult Visit(BoundBreakStatement statement);
    TResult Visit(BoundContinueStatement statement);
}
