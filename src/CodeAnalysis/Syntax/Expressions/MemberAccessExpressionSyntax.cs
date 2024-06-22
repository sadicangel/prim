
namespace CodeAnalysis.Syntax.Expressions;
public sealed record class MemberAccessExpressionSyntax(
    SyntaxTree SyntaxTree,
    ExpressionSyntax Expression,
    SyntaxToken OperatorToken,
    IdentifierNameExpressionSyntax Name)
    : ExpressionSyntax(SyntaxKind.MemberAccessExpression, SyntaxTree)
{
    public override IEnumerable<SyntaxNode> Children()
    {
        yield return Expression;
        yield return OperatorToken;
        yield return Name;
    }
}
