
namespace CodeAnalysis.Syntax.Expressions;
public record class IdentifierNameExpressionSyntax(
    SyntaxTree SyntaxTree,
    SyntaxToken IdentifierToken)
    : ExpressionSyntax(SyntaxKind.IdentifierNameExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return IdentifierToken;
    }
}
