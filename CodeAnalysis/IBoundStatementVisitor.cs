using CodeAnalysis.Binding;

namespace CodeAnalysis;

internal interface IBoundStatementVisitor
{
    void Accept(BoundBlockStatement statement);
    void Accept(BoundExpressionStatement statement);
}
