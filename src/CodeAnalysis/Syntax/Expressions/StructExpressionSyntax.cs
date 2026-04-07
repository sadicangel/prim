using CodeAnalysis.Syntax.Names;

namespace CodeAnalysis.Syntax.Expressions;

public sealed record class StructExpressionSyntax(
    NameSyntax StructName,
    SyntaxToken BraceOpenToken,
    SeparatedSyntaxList<PropertyExpressionSyntax> Properties,
    SyntaxToken BraceCloseToken)
    : ExpressionSyntax(SyntaxKind.StructExpression)
{
    /// <inheritdoc />
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return StructName;
        yield return BraceOpenToken;
        foreach (var property in Properties)
            yield return property;
        yield return BraceCloseToken;
    }
}
