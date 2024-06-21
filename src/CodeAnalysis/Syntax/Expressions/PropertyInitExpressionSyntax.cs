
namespace CodeAnalysis.Syntax.Expressions;

public sealed record class PropertyInitExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken DotToken,
    SyntaxToken IdentifierToken,
    SyntaxToken EqualsToken,
    ExpressionSyntax Init)
    : ExpressionSyntax(SyntaxKind.PropertyInitExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return DotToken;
        yield return IdentifierToken;
        yield return EqualsToken;
        yield return Init;
    }
}
