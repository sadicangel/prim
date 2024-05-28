
namespace CodeAnalysis.Syntax.Expressions;

public sealed record class MemberExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken DotToken,
    SyntaxToken IdentifierToken,
    SyntaxToken EqualsToken,
    ExpressionSyntax Expression)
    : SyntaxNode(SyntaxKind.MemberExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return DotToken;
        yield return IdentifierToken;
        yield return EqualsToken;
        yield return Expression;
    }
}
