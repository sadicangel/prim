
namespace CodeAnalysis.Syntax.Expressions;

public sealed record class PropertyExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken DotToken,
    SyntaxToken IdentifierToken,
    SyntaxToken EqualsToken,
    ExpressionSyntax Init)
    : SyntaxNode(SyntaxKind.PropertyExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return DotToken;
        yield return IdentifierToken;
        yield return EqualsToken;
        yield return Init;
    }
}
