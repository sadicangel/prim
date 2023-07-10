using CodeAnalysis.Binding.Statements;

namespace CodeAnalysis.Binding;

internal interface IBoundStatementVisitor<out TResult>
{
    TResult Visit(BoundVariableDeclaration declaration);
    TResult Visit(BoundFunctionDeclaration declaration);
    TResult Visit(BoundLabelDeclaration declaration);

    TResult Visit(BoundBlockStatement statement);
    TResult Visit(BoundExpressionStatement statement);
    TResult Visit(BoundIfStatement statement);
    TResult Visit(BoundWhileStatement statement);
    TResult Visit(BoundForStatement statement);
    TResult Visit(BoundGotoStatement statement);
    TResult Visit(BoundConditionalGotoStatement statement);
    TResult Visit(BoundReturnStatement statement);
}