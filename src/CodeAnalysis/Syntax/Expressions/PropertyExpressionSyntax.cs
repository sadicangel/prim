using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Syntax.Expressions;

public sealed record class PropertyExpressionSyntax(
    SimpleNameSyntax PropertyName,
    SyntaxToken EqualsToken,
    ExpressionSyntax Value)
    : SyntaxNode(SyntaxKind.PropertyExpression)
{
    /// <inheritdoc />
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return PropertyName;
        yield return EqualsToken;
        yield return Value;
    }
}
