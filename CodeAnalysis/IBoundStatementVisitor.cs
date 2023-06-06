using CodeAnalysis.Binding;

namespace CodeAnalysis;

internal interface IBoundStatementVisitor
{
    void Accept(BoundBlockStatement statement);
    void Accept(BoundDeclarationStatement statement);
    void Accept(BoundExpressionStatement statement);
    void Accept(BoundIfStatement statement);
    void Accept(BoundWhileStatement statement);
    void Accept(BoundForStatement statement);
}
