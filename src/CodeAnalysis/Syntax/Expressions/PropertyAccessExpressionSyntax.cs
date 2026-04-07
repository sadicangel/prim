using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Syntax.Expressions;

public sealed record class PropertyAccessExpressionSyntax(
    ExpressionSyntax Receiver,
    SyntaxToken DotToken,
    SimpleNameSyntax PropertyName)
    : ExpressionSyntax(SyntaxKind.PropertyAccessExpression)
{
    /// <inheritdoc />
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Receiver;
        yield return DotToken;
        yield return PropertyName;
    }
}
