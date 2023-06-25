namespace CodeAnalysis.Binding;

internal interface IBoundStatementVisitor
{
    void Visit(BoundBlockStatement statement);
    void Visit(BoundDeclarationStatement statement);
    void Visit(BoundExpressionStatement statement);
    void Visit(BoundIfStatement statement);
    void Visit(BoundWhileStatement statement);
    void Visit(BoundForStatement statement);
}
